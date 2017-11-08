using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Gms.Maps;
using Android.Widget;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Runtime;
using static Android.Gms.Maps.GoogleMap;
using Firebase.Auth;
using Android.Content;
using Firebase.Xamarin.Database;
using Firebase;
using Free_Room.Model;
using System.Collections.Generic;
using Plugin.Connectivity;

namespace Free_Room
{
    [Activity(Label = "FreeRoom", MainLauncher = false, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback, IInfoWindowAdapter, IOnInfoWindowClickListener
    {
        private GoogleMap mMap;
        private DrawerLayout activity_main;
        private DrawerLayout mDrawerLayout;
        private NavigationView navigationView;
        private View navHeader;
        private SupportToolbar toolBar;
        private SupportActionBar ab;
        private TextView loggedUser;
        private List<AptInfo> apartments;
        private List<Marker> markers;
        Marker aptLocationMarker;
        bool isSearchOn = false;
        Spinner city_spinner;
        //SEARCH
        XpandItComponents.RangeSliderView rangeSlider;
        private Button search_btn, clear_btn;
        private TextView min_price, max_price;
        private EditText search_bed, search_room;
        private int SEARCH_FLAG;
        FloatingActionButton fab;
        private struct indexes
        {
            private string marker_id;
            private int item_id;
            private indexes(string p1, int p2)
            {
                marker_id = p1;
                item_id = p2;
            }
        }

        public bool backpress = false;
        private const int MY_PERMISSION_REQUEST_CODE = 7171;
        private FloatingActionButton btnAddNewApt;
        private const string FirebaseURL = "https://freeroom-74739.firebaseio.com/";
        public string clicked_marker, selected_city;
        FirebaseAuth auth;




        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case MY_PERMISSION_REQUEST_CODE:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Toast.MakeText(this, "ODOBRENO", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "NIJE ODOBRENO", ToastLength.Short).Show();
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            //Init Firebase
            auth = FirebaseAuth.GetInstance(Login.app);
            if (auth != null)
            {
                FirebaseApp.InitializeApp(this);

                activity_main = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

                toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
                SetSupportActionBar(toolBar);
                ab = SupportActionBar;
                ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
                ab.SetDisplayHomeAsUpEnabled(true);

                //DrawerLayout/NavigationView/Header/Username
                mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                navHeader = navigationView.GetHeaderView(0);
                loggedUser = navHeader.FindViewById<TextView>(Resource.Id.navHeaderUser_TextView);
                loggedUser.Text = auth.CurrentUser.Email;
                if (navigationView != null)
                    SetUpDrawerContent(navigationView);

                SetUpMap();

                //SEARCH

                LinearLayout sheet = FindViewById<LinearLayout>(Resource.Id.bottomsheet);
                BottomSheetBehavior bottomSheetBehavior = BottomSheetBehavior.From(sheet);
                bottomSheetBehavior.PeekHeight = 0;
                bottomSheetBehavior.Hideable = true;
                bottomSheetBehavior.SetBottomSheetCallback(new Bottomcallback());

                fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                fab.Click += (o, e) =>
                {
                    if (bottomSheetBehavior.State == 3 || bottomSheetBehavior.State == 2)
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                        fab.SetImageResource(Resource.Drawable.search_icon);
                    }
                    if (bottomSheetBehavior.State == 4)
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                        fab.SetImageResource(Resource.Drawable.close_icon);
                    }
                };

                //CITY SPINNER
                city_spinner = FindViewById<Spinner>(Resource.Id.city_spinner);
                city_spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(City_spinner_ItemSelected);
                var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.city_array, Android.Resource.Layout.SimpleSpinnerItem);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                city_spinner.Adapter = adapter;
                //PRICE
                rangeSlider = new XpandItComponents.RangeSliderView(this);
                rangeSlider = FindViewById<XpandItComponents.RangeSliderView>(Resource.Id.range_slider);
                rangeSlider.RegisterComponent("stipep12@gmail.com", "z@pwlw89Gnthps5jvt");
                rangeSlider.OverlayLinePaint.Color = Color.Rgb(2, 136, 209);

                min_price = FindViewById<TextView>(Resource.Id.min_price);
                max_price = FindViewById<TextView>(Resource.Id.max_price);

                search_btn = FindViewById<Button>(Resource.Id.search_btn);
                clear_btn = FindViewById<Button>(Resource.Id.remove_filters_button);
                search_bed = FindViewById<EditText>(Resource.Id.num_of_beds);
                search_room = FindViewById<EditText>(Resource.Id.num_of_rooms);


                clear_btn.Click += (o, e) =>
                {

                    rangeSlider.SelectedMin = rangeSlider.AbsoluteMin;
                    rangeSlider.SelectedMax = rangeSlider.AbsoluteMax;
                    min_price.Text = rangeSlider.SelectedMin.ToString("MIN: ##0 kn");
                    max_price.Text = rangeSlider.SelectedMax.ToString("MAX: ##0 kn");
                    search_bed.Text = "";
                    search_room.Text = "";
                    city_spinner.SetSelection(0);
                    foreach (var mark in markers)
                        mark.Visible = true;
                    isSearchOn = false;
                };
                search_btn.Click += (o, e) =>
                {
                    if (markers.Count != 0)
                    {
                        int s_price;
                        int search_counter = 0;

                        foreach (var apt in apartments)
                        {
                            SEARCH_FLAG = 0;

                            if (selected_city != "Odaberi grad")
                            {
                                if (selected_city.ToUpper() != apt.aptCity.ToUpper())
                                {
                                    SEARCH_FLAG = 1;
                                }
                            }

                            if (Int32.TryParse(apt.aptPrice, out s_price))
                            {
                                if (s_price < (int)rangeSlider.SelectedMin || s_price > (int)rangeSlider.SelectedMax)
                                    SEARCH_FLAG = 1;
                            }
                            if (search_bed.Text != "")
                            {
                                int s_bed, a_bed;
                                if (Int32.TryParse(apt.aptNumOfBeds, out a_bed))
                                {
                                    if (Int32.TryParse(search_bed.Text, out s_bed))
                                        if (a_bed != s_bed)
                                            SEARCH_FLAG = 1;
                                }
                            }

                            if (search_room.Text != "")
                            {
                                int s_room, a_room;
                                if (Int32.TryParse(apt.aptNumOfRooms, out a_room))
                                {
                                    if (Int32.TryParse(search_bed.Text, out s_room))
                                        if (s_room != a_room)
                                            SEARCH_FLAG = 1;
                                }
                            }


                            foreach (var mark in markers)
                            {
                                if (mark.Id == apt.aptMarkerId)
                                {
                                    if (SEARCH_FLAG == 1)
                                        mark.Visible = false;
                                    else
                                    {
                                        mark.Visible = true;
                                        search_counter++;
                                    }
                                }
                            }

                        }
                        if (search_counter == 0)
                            Toast.MakeText(this, "Nijedan oglas ne odgovara vašem pretraživanju!", ToastLength.Short).Show();
                        else if (search_counter == 1)
                            Toast.MakeText(this, "Pronađen 1 oglas!", ToastLength.Short).Show();
                        else
                            Toast.MakeText(this, "Pronađeno " + Convert.ToString(search_counter) + " oglasa!", ToastLength.Short).Show();
                        bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                        fab.SetImageResource(Resource.Drawable.search_icon);
                        isSearchOn = true;
                    }
                    else
                    {
                        Toast.MakeText(this, "Smještaji još nisu učitani", ToastLength.Short).Show();
                    }
                };

                //PRICE RANGE CHANGE LISTENER
                rangeSlider.RangeChanged += (sender, args) =>
                {
                    min_price.Text = rangeSlider.SelectedMin.ToString("MIN: ##0 kn");
                    max_price.Text = rangeSlider.SelectedMax.ToString("MAX: ##0 kn");
                };

                //ADD NEW
                btnAddNewApt = FindViewById<FloatingActionButton>(Resource.Id.fab_add);
                btnAddNewApt.Click += delegate
                {
                    var activityadd = new Intent(this, typeof(AddAptDataActivity));
                    Bundle extras = new Bundle();
                    extras.PutBoolean("main", true);
                    activityadd.PutExtras(extras);
                    StartActivity(activityadd);
                };
            }
            else
            {
                Toast.MakeText(this, "Odjavljeni ste, molimo prijavite se ponovno!", ToastLength.Short).Show();
                Finish();
                StartActivity(typeof(Login));
            }

        }

        private async System.Threading.Tasks.Task LoadData()
        {
            if (mMap!=null)
            {
                mMap.Clear();
            }
            if (auth != null)
            {
                var connection = CrossConnectivity.Current.IsConnected;
                if (connection)
                {
                    

                    int min, max, price;
                    min = int.MaxValue;
                    max = int.MinValue;

                    var firebase = new FirebaseClient(FirebaseURL);
                    var items = await firebase
                        .Child("apartments")
                        .OnceAsync<AptInfo>();
                    markers = new List<Marker>();
                    apartments = new List<AptInfo>();
                    foreach (var item in items)
                    {

                        AptInfo apartment = new AptInfo
                        {
                            aptCity = item.Object.aptCity,
                            aptContact = item.Object.aptContact,
                            aptDescription = item.Object.aptDescription,
                            aptImageId = item.Object.aptImageId,
                            aptKlima = item.Object.aptKlima,
                            aptWifi = item.Object.aptWifi,
                            aptTV = item.Object.aptTV,
                            aptParking = item.Object.aptParking,
                            aptPrice = item.Object.aptPrice,
                            aptSelectedAddress = item.Object.aptSelectedAddress,
                            aptNumOfRooms = item.Object.aptNumOfRooms,
                            aptNumOfBeds = item.Object.aptNumOfBeds,
                            aptNumOfPerson = item.Object.aptNumOfPerson,
                            aptName = item.Object.aptName,
                            aptLat = item.Object.aptLat,
                            aptLng = item.Object.aptLng
                        };

                        if (Int32.TryParse(item.Object.aptPrice, out price))
                        {
                            if (price > max)
                                max = price;
                            if (price < min)
                                min = price;
                        }
                        rangeSlider.AbsoluteMin = min;
                        rangeSlider.AbsoluteMax = max;
                        min_price.Text = "MIN:" + Convert.ToString(min) + " kn";
                        max_price.Text = "MAX:" + Convert.ToString(max) + " kn";
                        aptLocationMarker = mMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(apartment.aptLat,
                        apartment.aptLng)).SetTitle(apartment.aptName).SetSnippet(apartment.aptPrice + " kn"));
                        aptLocationMarker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.bed_icon));

                        apartment.aptMarkerId = aptLocationMarker.Id;
                        apartments.Add(apartment);
                        markers.Add(aptLocationMarker);
                    }
                }
                else
                {
                    Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Odjavljeni ste, molimo prijavite se ponovno!", ToastLength.Short).Show();
                Finish();
                StartActivity(typeof(Login));
            }
        }
        private void LogoutUser()
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(Login)));
                Finish();
            }
            else
            {
                Toast.MakeText(this, "Problem s izlaskom. Pokušajte ponovo!", ToastLength.Short).Show();
            }
        }
        private void City_spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            selected_city = string.Format("{0}", spinner.GetItemAtPosition(e.Position));

            switch (selected_city)
            {

                case "Dubrovnik":
                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(42.6505556, 18.0913889));
                    builder.Zoom(12);
                    CameraPosition cameraPosition = builder.Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
                case "Split":
                    builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(43.508133, 16.440193));
                    builder.Zoom(12);
                    cameraPosition = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
                case "Zadar":
                    builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(44.1197222, 15.2422222));
                    builder.Zoom(12);
                    cameraPosition = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
                case "Rijeka":
                    builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(45.328979, 14.457664));
                    builder.Zoom(12);
                    cameraPosition = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
                case "Zagreb":
                    builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(45.815399, 15.966568));
                    builder.Zoom(11);
                    cameraPosition = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
                case "Osijek":
                    builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(45.5511111, 18.6938889));
                    builder.Zoom(12);
                    cameraPosition = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
                default:
                    builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(44.5, 16.3));
                    builder.Zoom((float)6.5);
                    cameraPosition = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    mMap.MoveCamera(cameraUpdate);
                    break;
            }

        }

        public void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                e.MenuItem.SetCheckable(false);
                mDrawerLayout.CloseDrawers();
                if (e.MenuItem.ItemId == Resource.Id.nav_myrooms)
                {
                    StartActivity(typeof(MyRoomsActivity));
                    Finish();
                }
                if (e.MenuItem.ItemId == Resource.Id.nav_logout)
                {
                    LogoutUser();
                }
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                case Resource.Id.refresh_btn:
                    Refresh();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private async void Refresh()
        {


            if (isSearchOn)
            {
                rangeSlider.SelectedMin = rangeSlider.AbsoluteMin;
                rangeSlider.SelectedMax = rangeSlider.AbsoluteMax;
                min_price.Text = rangeSlider.SelectedMin.ToString("MIN: ##0 kn");
                max_price.Text = rangeSlider.SelectedMax.ToString("MAX: ##0 kn");
                search_bed.Text = "";
                search_room.Text = "";
                city_spinner.SetSelection(0);
                foreach (var mark in markers)
                    mark.Visible = true;
                isSearchOn = false;
            }
            await LoadData();

        }
        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            if (mMap != null)
            {
                await LoadData();
                mMap.SetInfoWindowAdapter(this);
                mMap.SetOnInfoWindowClickListener(this);
                mMap.MyLocationEnabled = true;
            }
        }
        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        public View GetInfoWindow(Marker marker)
        {
                View view = LayoutInflater.Inflate(Resource.Layout.info_window, null, false);
                view.FindViewById<TextView>(Resource.Id.info_name).Text = marker.Title;
                view.FindViewById<TextView>(Resource.Id.info_price).Text = marker.Snippet;
                return view;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            clicked_marker = marker.Id;

            foreach (var apt in apartments)
            {
                string wifi, tv, klima, parking;
                if (apt.aptMarkerId == clicked_marker)
                {
                    if (apt.aptWifi == true)
                        wifi = "DA";
                    else
                        wifi = "NE";

                    if (apt.aptKlima == true)
                        klima = "DA";
                    else
                        klima = "NE";

                    if (apt.aptParking == true)
                        parking = "DA";
                    else
                        parking = "NE";

                    if (apt.aptTV == true)
                        tv = "DA";
                    else
                        tv = "NE";

                    var activity2 = new Intent(this, typeof(Activity_accomodation_mainpage));
                    Bundle extras = new Bundle();
                    extras.PutString("name", apt.aptName);
                    extras.PutString("beds", apt.aptNumOfBeds);
                    extras.PutString("rooms", apt.aptNumOfRooms);
                    extras.PutString("person", apt.aptNumOfPerson);
                    extras.PutString("price", apt.aptPrice);
                    extras.PutString("contact", apt.aptContact);
                    extras.PutString("city", apt.aptCity);
                    extras.PutString("adress", apt.aptSelectedAddress);
                    extras.PutString("description", apt.aptDescription);
                    extras.PutString("wifi", wifi);
                    extras.PutString("klima", klima);
                    extras.PutString("tv", tv);
                    extras.PutString("parking", parking);
                    extras.PutString("imageid", apt.aptImageId);
                    extras.PutDouble("lat", apt.aptLat);
                    extras.PutDouble("lng", apt.aptLng);
                    extras.PutBoolean("main", true);
                    activity2.PutExtras(extras);
                    StartActivity(activity2);
                    Finish();

                }
            }
        }
        public class Bottomcallback : BottomSheetBehavior.BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float slideOffset)
            {
                //Slide               
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                BottomSheetBehavior bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
                if (newState == BottomSheetBehavior.StateDragging)
                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                if (newState == 2)
                    bottomSheetBehavior.State = 3;
            }
        }

        public override void OnBackPressed()
        {
            if (backpress == false)
            {
                Toast.MakeText(this, "Pritisnite opet tipku za natrag ukoliko želite izaći iz aplikacije.", ToastLength.Short).Show();
                backpress = true;
            }
            else if (backpress == true)
            {
                backpress = false;
                Finish();
            }
            new Handler().PostDelayed(() =>
            {
                backpress = false;
            }, 2000);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {

            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }


    }
}