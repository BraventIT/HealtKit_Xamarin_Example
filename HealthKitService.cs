using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;

namespace HealthKitPoc
{
    public class HealthKitService
    {
        private HKHealthStore HealthStore;
		private List<WeightData> data;
		private List<HKSample> results;

		NSSet DataTypesToWrite
		{
			get
			{
                return NSSet.MakeNSObjectSet<HKObjectType>(new HKObjectType[] {
                    HKQuantityType.Create (HKQuantityTypeIdentifier.BodyMass),
                    HKObjectType.GetWorkoutType()
				});
			}
		}

		NSSet DataTypesToRead
		{
			get
			{
				return NSSet.MakeNSObjectSet<HKObjectType>(new HKObjectType[] {
					HKQuantityType.Create (HKQuantityTypeIdentifier.BodyMass),
					HKObjectType.GetWorkoutType()
				});
			}
		}

        public HealthKitService()
        {
            HealthStore = new HKHealthStore();
        }

        public async Task Init(){
			if (HKHealthStore.IsHealthDataAvailable)
			{

				var success = await HealthStore.RequestAuthorizationToShareAsync(DataTypesToWrite, DataTypesToRead);

				/*if (!success.Item1)
				{
					Console.WriteLine("You didn't allow HealthKit to access these read/write data types. " +
					"In your app, try to handle this error gracefully when a user decides not to provide access. " +
					"If you're using a simulator, try it on a device.");
					return;
				}*/

			}
        }

		private void FetchMostRecentData(HKQuantityType quantityType, Action<List<HKSample>, NSError> completion,int days)
		{

			var timeSortDescriptor = new NSSortDescriptor(HKSample.SortIdentifierEndDate, false);
			DateTime startDay = DateTime.Now;
			DateTime endDay = startDay.AddDays(-days).AddHours(-startDay.Hour).AddMinutes(-startDay.Minute).AddSeconds(-startDay.Second).AddMilliseconds(-startDay.Millisecond);
			var predicate = HKQuery.GetPredicateForSamples(DateUtil.DateTimeToNSDate(endDay), DateUtil.DateTimeToNSDate(startDay), HKQueryOptions.None);
			var query = new HKSampleQuery(quantityType, predicate, 0, new NSSortDescriptor[] { timeSortDescriptor },
								(HKSampleQuery resultQuery, HKSample[] results, NSError error) =>
								{
									if (completion != null && error != null)
									{
										completion(null, error);
										return;
									}

									completion?.Invoke(results.ToList(), error);
								});

			HealthStore.ExecuteQuery(query);

		}

        public Task<List<WeightData>> GetWeigths(){
            var weightType = HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass);
            if (HealthStore.GetAuthorizationStatus(weightType) == HKAuthorizationStatus.SharingAuthorized)
            {
                var completionSource = new TaskCompletionSource<List<WeightData>>();
                FetchMostRecentData(weightType, (QuantityResults, error) =>
                {
                    if (error != null)
                    {
                        //Console.WriteLine("An error occured fetching the user's age information. " +
                        //"In your app, try to handle this gracefully. The error was: {0}", error.LocalizedDescription);
                        completionSource.SetResult(null);
                    }

                    data = new List<WeightData>();
                    if (QuantityResults != null)
                    {
                        results = QuantityResults;
                        for (int i = 0; i < QuantityResults.Count; i++)
                        {
                            WeightData h = new WeightData();
                            var weightUnit = HKUnit.Gram;
                            h.Value = ((HKQuantitySample)QuantityResults[i]).Quantity.GetDoubleValue(weightUnit) / 1000f;
                            h.Unit = "kg";
                            h.Date = DateUtil.NSDateToDateTime(((HKQuantitySample)QuantityResults[i]).StartDate);
                            data.Add(h);
                        }
                    }
                    completionSource.SetResult(data);
                }, 365 * 2
                );
                return completionSource.Task;
            }
            else
                return null;
        }

        public Task<bool> SaveWeightIntoHealthStore(WeightData weight)
		{
            var weightQuantity = HKQuantity.FromQuantity(HKUnit.Gram, weight.Value * 1000);
			var weightType = HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass);
            var weightSample = HKQuantitySample.FromType(weightType, weightQuantity, DateUtil.DateTimeToNSDate(weight.Date), DateUtil.DateTimeToNSDate(weight.Date), new NSDictionary());
            var completionSource = new TaskCompletionSource<bool>();
			HealthStore.SaveObject(weightSample, (success, error) =>
			{
                if (!success)
                {
                    completionSource.SetResult(false);
                }
                else
                {
                    completionSource.SetResult(true);
                }
			});
            return completionSource.Task;
		}

        public Task<bool> DeleteWeightIntoHealthStore(WeightData weight)
		{
            int index = data.FindIndex(x => Math.Abs(x.Value - weight.Value) < 0.01 && weight.Date.Ticks == x.Date.Ticks);
			var sample = results[index];
            var completionSource = new TaskCompletionSource<bool>();
			HealthStore.DeleteObject(sample, (success, error) =>
			{
				if (!success)
				{
                    /*InvokeOnMainThread(delegate
				{
					new UIAlertView("HealthKit", "No puede borrar este registro, ha sido creado por otra app", null, "OK", null).Show();
				});
					Console.WriteLine("An error occured saving the weight sample {0}. " +
						"In your app, try to handle this gracefully. The error was: {1}.", sample, error.LocalizedDescription);
					return;*/
                    completionSource.SetResult(false);
                }else{
                    completionSource.SetResult(true);
                }
                			
			});
            return completionSource.Task;
		}

        public Task <ExerciseData> AddExercise (ExerciseData data)
        {
            var completionSource = new TaskCompletionSource<ExerciseData>();
			var metadata = new HKMetadata()
			{
				GroupFitness = true,
				IndoorWorkout = true,
				CoachedWorkout = true,

			};
			if (data.Time.TotalMinutes == 0)
			{ 
				DateTime start = ((DateTime)data.DateInit).ToLocalTime();
				DateTime end = ((DateTime)data.DateEnd).ToLocalTime();

				data.Time= end.Subtract(start);
			}
            HKQuantity calories = getCaloriesFromData(data);

            HKWorkout workOut = HKWorkout.Create(HKWorkoutActivityType.TraditionalStrengthTraining,
                                                 data.DateInit,
                                                 data.DateEnd,
                                                 null,
                                                 calories,
                                                 null,
                                                 metadata);
           
            HealthStore.SaveObject(workOut,(succes, error) => {

                if (succes)
                {
                    data.Kilocalories = calories.GetDoubleValue(HKUnit.Kilocalorie);

                    completionSource.SetResult(data);
                }
                else
                {
                    completionSource.SetResult(null);
                }
            });

            return completionSource.Task;
        }

        private HKQuantity getCaloriesFromData(ExerciseData data)
        {
            //0,029 x(peso corporal en kg) x 2,2 x Total de minutos practicados = cantidad de calorías aproximadas quemadas
            double userWeight = 88.0;
            double weigthEdit = 0.029;
            double timeEdit = 2.2;

            var kilocalories = weigthEdit * userWeight;
			kilocalories = kilocalories * (timeEdit * data.Time.TotalMinutes);

            kilocalories = Math.Round(kilocalories, 2);

            return HKQuantity.FromQuantity(HKUnit.Kilocalorie, kilocalories);
        }
    }
}
