using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using UIKit;
using static HealthKitPoc.WeightController;

namespace HealthKitPoc
{
    public partial class ViewController : UIViewController
    {

        HealthKitService service;
        private List<WeightData> data;


        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
            addButton.TouchUpInside+=delegate {

                UIAlertController cont = UIAlertController.Create("Añadir", "¿Que quieres añadir?", UIAlertControllerStyle.ActionSheet);
                cont.AddAction(UIAlertAction.Create("Peso",UIAlertActionStyle.Default,(obj) => {
					WeightController newController = (WeightController)this.Storyboard.InstantiateViewController("weightID");
					WeightController.Weight = null;
					newController.OnAddWeight += async (sender, args) =>
					{
						//SaveWeightIntoHealthStore(args.Weight.Value,args.Weight.Date);    
						await service.SaveWeightIntoHealthStore(args.Weight);
						UpdateUsersWeight();
					};
					PresentViewController(newController, true, null);
                }));

                cont.AddAction(UIAlertAction.Create("Ejercicio",UIAlertActionStyle.Default,(obj) => {
                    PresentViewController(new AddExerciseViewController(),true,null);

                }));

                PresentViewController(cont,true,null);
			};
			
		
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            service = new HealthKitService();
            await service.Init();
            UpdateUsersWeight();
		}


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        async Task UpdateUsersWeight()
        {
            data = await service.GetWeigths();
            if (data != null)
            {
                InvokeOnMainThread(delegate
                {
                    TableWeight.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                    TableWeight.Hidden = true;
                    WeightTableSource source = new WeightTableSource(data);
                    TableWeight.Source = source;
                    source.OnWeightSelected += (sender, args) =>
                    {
                        WeightController newController = (WeightController)this.Storyboard.InstantiateViewController("weightID");
                        WeightController.Weight = args.Weight;
                        newController.OnDeleteWeight += async (sender2, args2) =>
                        {
                            if (await service.DeleteWeightIntoHealthStore(args2.Weight))
                                UpdateUsersWeight();
                            else
                                InvokeOnMainThread(delegate
                                    {
                                        new UIAlertView("HealthKit", "No puede borrar este registro, ha sido creado por otra app", null, "OK", null).Show();
                                    });
                        };
                        PresentViewController(newController, true, null);
                    };
                    TableWeight.ReloadData();
                    TableWeight.Hidden = false;
                    WeightGraph.SetData(data, 75, 66);
                    //WeightGraph.SetData(data, 95, 96);
                });
            }
        }


		public class WeightTableSource : UITableViewSource
		{

            public event WeightHandler OnWeightSelected;

            private List<WeightData> data;
			//public event MedicionHandler OnMedicion;

            public WeightTableSource(List<WeightData> data)
			{
				this.data = data;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
                return new nint(data.Count);
			}
			public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				WeightTableViewCell cell = WeightTableViewCell.Create();
				cell.SetWeight(data[indexPath.Row], indexPath.Row);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				return cell;
			}

			public override nint NumberOfSections(UITableView tableView)
			{
				return 1;
			}

			public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				return 60;
			}


			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				tableView.DeselectRow(indexPath, true);
                OnWeightSelected?.Invoke(this,new WeightEventArgs(data[indexPath.Row]));

			}
		}
    }
}
