using System;
using HealthKit;
using Foundation;

namespace HealthKitPoc
{
	public class GenericEventArgs<T> : EventArgs
	{
		public T Value { get; protected set; }
		public DateTime Time { get; protected set; }

		public GenericEventArgs(T value)
		{
			this.Value = value;
			Time = DateTime.Now;
		}
	}

	public delegate void GenericEventHandler<T>(object sender, GenericEventArgs<T> args);

	public sealed class BodyMassModel : NSObject
	{
		private static volatile BodyMassModel singleton;
		private static object syncRoot = new Object();

		private BodyMassModel()
		{
		}

		public static BodyMassModel Instance
		{
			get
			{
				//Double-check lazy initialization
				if (singleton == null)
				{
					lock (syncRoot)
					{
						if (singleton == null)
						{
							singleton = new BodyMassModel();
						}
					}
				}

				return singleton;
			}
		}

		private bool enabled = false;

		public event GenericEventHandler<bool> EnabledChanged;
		public event GenericEventHandler<String> ErrorMessageChanged;
		public event GenericEventHandler<Double> HeartRateStored;

		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
                    InvokeOnMainThread(() => EnabledChanged?.Invoke(this, new GenericEventArgs<bool>(value)));
				}
			}
		}

		public void PermissionsError(string msg)
		{
			Enabled = false;
            InvokeOnMainThread(() => ErrorMessageChanged?.Invoke(this, new GenericEventArgs<string>(msg)));
		}

		//Converts its argument into a strongly-typed quantity representing the value in beats-per-minute
		public HKQuantity HeartRateInBeatsPerMinute(ushort beatsPerMinute)
		{
			var heartRateUnitType = HKUnit.Count.UnitDividedBy(HKUnit.Minute);
			var quantity = HKQuantity.FromQuantity(heartRateUnitType, beatsPerMinute);

			return quantity;
		}

		public void StoreHeartRate(HKQuantity quantity)
		{
			var bpm = HKUnit.Count.UnitDividedBy(HKUnit.Minute);
			//Confirm that the value passed in is of a valid type (can be converted to beats-per-minute)
			if (!quantity.IsCompatible(bpm))
			{
				InvokeOnMainThread(() => ErrorMessageChanged(this, new GenericEventArgs<string>("Units must be compatible with BPM")));
			}

			var heartRateQuantityType = HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate);
			var heartRateSample = HKQuantitySample.FromType(heartRateQuantityType, quantity, new NSDate(), new NSDate(), new HKMetadata());

			using (var healthKitStore = new HKHealthStore())
			{
				healthKitStore.SaveObject(heartRateSample, (success, error) =>
				{
					InvokeOnMainThread(() =>
					{
						if (success)
						{
                            HeartRateStored?.Invoke(this, new GenericEventArgs<Double>(quantity.GetDoubleValue(bpm)));
						}
						else
						{
							ErrorMessageChanged(this, new GenericEventArgs<string>("Save failed"));
						}
						if (error != null)
						{
							//If there's some kind of error, disable 
							Enabled = false;
							ErrorMessageChanged(this, new GenericEventArgs<string>(error.ToString()));
						}
					});
				});
			}
		}
	}
}
