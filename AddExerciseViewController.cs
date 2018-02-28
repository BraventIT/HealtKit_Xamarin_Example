using System;
using Foundation;
using UIKit;

namespace HealthKitPoc
{
    public partial class AddExerciseViewController : UIViewController
    {
        NSDate end;
        NSDate start;
        public AddExerciseViewController() : base("AddExerciseViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NSDateFormatter formater = new NSDateFormatter();
            formater.DateFormat = "dd/MM/yyyy hh:MM";

            UIDatePicker datePickerInit = new UIDatePicker();
            datePickerInit.Mode = UIDatePickerMode.DateAndTime;
            datePickerInit.ValueChanged +=delegate {
                dateInit.Text = formater.ToString(datePickerInit.Date);
                start = datePickerInit.Date;
            };
            dateInit.InputView = datePickerInit;

			UIDatePicker datePickerEnd = new UIDatePicker();
			datePickerEnd.Mode = UIDatePickerMode.DateAndTime;
			datePickerEnd.ValueChanged += delegate
			{
                dateEnd.Text = formater.ToString(datePickerEnd.Date);
                end = datePickerEnd.Date;
			};
			dateEnd.InputView = datePickerEnd;

            HealthKitService healtService = new HealthKitService();

            btnSabe.TouchUpInside += async delegate
            {

                if (end != null && start != null)
                {
                    ExerciseData data = new ExerciseData();
                    data.DateEnd = end;
                    data.DateInit = start;
                    ExerciseData saveData = await healtService.AddExercise(data);
                    if (saveData!=null)
                    {
                        txfData.Text = string.Format("Kilocalorias consumidas = {0}",saveData.Kilocalories);
                    }
                    else
                    {
						UIAlertController error = UIAlertController.Create("Error", "Se ha producido un error al guardar los datos", UIAlertControllerStyle.Alert);
						PresentViewController(error, true, null);
                    }
                }
                else
                {
                    UIAlertController error = UIAlertController.Create("Error", "Introduce dos fechas", UIAlertControllerStyle.Alert);
                    PresentViewController(error, true, null);
                }

            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

