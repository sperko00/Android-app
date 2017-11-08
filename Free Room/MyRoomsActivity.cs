using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;
using Firebase.Xamarin.Database;
using Free_Room.Model;
using Firebase.Auth;
using Android.Widget;
using Firebase.Xamarin.Database.Query;
using Android.Content;
using Plugin.Connectivity;

namespace Free_Room
{
    [Activity(Label = "My Rooms", MainLauncher = false, Theme = "@style/AppTheme")]
    public class MyRoomsActivity : AppCompatActivity
    {
        private RecyclerView mRecyclerView;
        private RecyclerView.LayoutManager mLayoutManager;
        private RecyclerAdapter mAdapter;
        private List<Room> mRooms;
        private SupportToolbar myRoomsToolbar;
        private const string FirebaseURL = "https://freeroom-74739.firebaseio.com/";
        private FirebaseAuth auth;
        ProgressBar progress_bar;
        
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.myRooms);
            progress_bar = FindViewById<ProgressBar>(Resource.Id.progress_bar_myrooms);
            progress_bar.Visibility = ViewStates.Visible;
            var connection = CrossConnectivity.Current.IsConnected;
            if (connection)
            {
                auth = FirebaseAuth.GetInstance(Login.app);
                var fBase = new FirebaseClient(FirebaseURL);
                var items = await fBase
                    .Child("apartments")
                    .OnceAsync<AptInfo>();
                mRooms = new List<Room>();
                myRoomsToolbar = FindViewById<SupportToolbar>(Resource.Id.myRooms_toolbar);
                SetSupportActionBar(myRoomsToolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(true);
                Title = "Moje sobe";

                mRecyclerView = FindViewById<RecyclerView>(Resource.Id.myRoomsRV);
                mLayoutManager = new LinearLayoutManager(this);
                mRecyclerView.SetLayoutManager(mLayoutManager);
                mAdapter = new RecyclerAdapter(mRooms, mRecyclerView, this);
                mRecyclerView.SetAdapter(mAdapter);
                if (items.Count == 0)
                    Toast.MakeText(this, "Niste oglasili ni jedan smještaj. Za dodavanje kliknite znak + ", ToastLength.Short).Show();
                //Učitavanje soba
                foreach (var item in items)
                {

                    if (item.Object.aptUid == auth.CurrentUser.Uid)
                    {
                        mRooms.Add(new Room(item.Object.aptName, item.Object.aptSelectedAddress, double.Parse(item.Object.aptPrice), item.Key));
                        mAdapter.NotifyItemInserted(mRooms.Count - 1);
                    }
                }
                progress_bar.Visibility = ViewStates.Invisible;

                mAdapter.BrisanjeKliknuto += (o, e) =>
                {
                    progress_bar.Visibility = ViewStates.Visible;
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("Potvrdite brisanje");
                    alert.SetMessage("Da li želite izbrisati sobu?");
                    alert.SetPositiveButton("Da", async (senderAlert, args) =>
                    {
                        //Traženje rednog broja elementa i brisanje iz liste soba
                        int pozicija = mRooms.FindIndex(a => a.RUID == e.RUID);
                        mRooms.RemoveAt(pozicija);
                        //Brisanje iz baze
                        await fBase.Child("apartments").Child(e.RUID).DeleteAsync();
                        //Obnavljanje RecyclerViewa
                        mAdapter.NotifyItemRemoved(pozicija);
                    });
                    alert.SetNegativeButton("Ne", (senderAlert, args) =>
                    {
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                    progress_bar.Visibility = ViewStates.Invisible;
                };

                mAdapter.PromjenaKliknuta += (o, e) =>
                  {
                      EditText unosCijene = new EditText(this)
                      {
                          InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber
                      };

                      Android.Support.V7.App.AlertDialog.Builder promjenaCijene = new Android.Support.V7.App.AlertDialog.Builder(this);
                      promjenaCijene.SetTitle("Promjena cijene");
                      promjenaCijene.SetMessage("Unesite novu cijenu sobe");
                      promjenaCijene.SetView(unosCijene);
                      promjenaCijene.SetPositiveButton("Da", (senderAlert, args) =>
                       {
                           if (unosCijene.Text != string.Empty)
                           {
                               int pozicija = mRooms.FindIndex(a => a.RUID == e.RUID);
                               mRooms[pozicija].Cijena = double.Parse(unosCijene.Text);
                               PromjenaCijene(e.RUID, double.Parse(unosCijene.Text));
                               mAdapter.NotifyItemChanged(pozicija);
                           }
                           else
                               Toast.MakeText(this, "Cijena je ostala nepromijenjena!", ToastLength.Long).Show();
                       });
                      promjenaCijene.SetNegativeButton("Ne", (senderAlert, args) =>
                       {
                       });
                      Dialog dialog = promjenaCijene.Create();
                      dialog.Show();
                  };

                mAdapter.PregledKliknuto += async (o, e) =>
                  {
                      progress_bar.Visibility = ViewStates.Visible;
                      int pozicija = mRooms.FindIndex(a => a.RUID == e.RUID);
                      items = await fBase.Child("apartments").OnceAsync<AptInfo>();
                      foreach (var item in items)
                      {
                          if (item.Key == e.RUID)
                          {
                              var activity2 = new Intent(this, typeof(Activity_accomodation_mainpage));
                              Bundle extras = new Bundle();
                              extras.PutString("name", item.Object.aptName);
                              extras.PutString("beds", item.Object.aptNumOfBeds);
                              extras.PutString("rooms", item.Object.aptNumOfRooms);
                              extras.PutString("person", item.Object.aptNumOfPerson);
                              extras.PutString("price", item.Object.aptPrice);
                              extras.PutString("contact", item.Object.aptContact);
                              extras.PutString("city", item.Object.aptCity);
                              extras.PutString("adress", item.Object.aptSelectedAddress);
                              extras.PutString("description", item.Object.aptDescription);
                              extras.PutString("wifi", item.Object.aptWifi ? "DA" : "NE");
                              extras.PutString("klima", item.Object.aptKlima ? "DA" : "NE");
                              extras.PutString("tv", item.Object.aptTV ? "DA" : "NE");
                              extras.PutString("parking", item.Object.aptParking ? "DA" : "NE");
                              extras.PutString("imageid", item.Object.aptImageId);
                              extras.PutDouble("lat", item.Object.aptLat);
                              extras.PutDouble("lng", item.Object.aptLng);

                              activity2.PutExtras(extras);
                              StartActivity(activity2);
                              progress_bar.Visibility = ViewStates.Invisible;

                          }
                      }
                  };
            }
            else
            {
                Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
                progress_bar.Visibility = ViewStates.Invisible;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MyRoomsActivityActionBar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //Tipka za nazad i dodavanje
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    StartActivity(typeof(MainActivity));
                    Finish();
                    return true;
                case Resource.Id.add:
                    StartActivity(typeof(AddAptDataActivity));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }

        public async void PromjenaCijene(string rUIDsobe, double novaCijena)
        {
            var fBase = new FirebaseClient(FirebaseURL);
            var sobe = await fBase
                .Child("apartments")
                .OnceAsync<AptInfo>();

            foreach (var soba in sobe)
            {
                if (soba.Key == rUIDsobe)
                {
                    AptInfo promjenjena = new AptInfo()
                    {
                        aptUid = soba.Object.aptUid,
                        aptName = soba.Object.aptName,
                        aptCity = soba.Object.aptCity,
                        aptNumOfRooms = soba.Object.aptNumOfRooms,
                        aptNumOfBeds = soba.Object.aptNumOfBeds,
                        aptDescription = soba.Object.aptDescription,
                        aptPrice = novaCijena.ToString(),
                        aptNumOfPerson = soba.Object.aptNumOfPerson,
                        aptContact = soba.Object.aptContact,
                        aptSelectedAddress = soba.Object.aptSelectedAddress,
                        aptLat = soba.Object.aptLat,
                        aptLng = soba.Object.aptLng,
                        aptImageId = soba.Object.aptImageId,
                        aptKlima = soba.Object.aptKlima,
                        aptParking = soba.Object.aptParking,
                        aptTV = soba.Object.aptTV,
                        aptWifi = soba.Object.aptWifi,
                        aptKey = soba.Object.aptKey,
                        aptMarkerId = soba.Object.aptMarkerId
                    };
                    await fBase.Child("apartments").Child(rUIDsobe).PutAsync<AptInfo>(promjenjena);
                    Toast.MakeText(this, "Cijena je uspješno promjenjena.", ToastLength.Short).Show();
                }
            }
        }
    }
}