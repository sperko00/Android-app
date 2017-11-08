using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using static Android.Views.View;
using Android.Gms.Tasks;
using Firebase.Auth;
using Firebase;
using Android.Views.InputMethods;
using System.Net.Mail;
using Plugin.Connectivity;

namespace Free_Room
{
    [Activity(Label = "Free Room", MainLauncher = true, Icon = "@drawable/logo_ver_4", Theme = "@style/LoginTheme")]
    public class Login : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        private Button btnLogin;
        private EditText input_email, input_password;
        private TextView btnSignUp, btnForgotPassword;
        private InputMethodManager imm;
        private LinearLayout activity_main;
        public bool isOnline;
        public static FirebaseApp app;
        private FirebaseAuth auth;
        ImageView logo_img;
        private ProgressBar progress_bar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Login);

            //Init Firebase
            InitFirebaseAuth();

            //View
            imm = (InputMethodManager)GetSystemService(InputMethodService);
            btnLogin = FindViewById<Button>(Resource.Id.login_btn_login);
            input_email = FindViewById<EditText>(Resource.Id.login_email);
            input_password = FindViewById<EditText>(Resource.Id.login_password);
            btnSignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
            btnForgotPassword = FindViewById<TextView>(Resource.Id.login_btn_forgot_password);
            activity_main = FindViewById<LinearLayout>(Resource.Id.activity_main);
            progress_bar = FindViewById<ProgressBar>(Resource.Id.progress_bar_login);
            logo_img = FindViewById<ImageView>(Resource.Id.icon);
            btnSignUp.SetOnClickListener(this);
            btnLogin.SetOnClickListener(this);
            btnForgotPassword.SetOnClickListener(this);
        }

        private static bool IsValidEmail(string EmailToCheck)
        {
            try
            {
                MailAddress mail = new MailAddress(EmailToCheck);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder()
              .SetApplicationId("1:100910522351:android:e0c52bc71dcd6b35")
              .SetApiKey("AIzaSyAKoI_vbbPVmQcCsxVW2guioJunA3dPGbg")
              .Build();

            if (app == null)
                app = FirebaseApp.InitializeApp(this, options);
            auth = FirebaseAuth.GetInstance(app);
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.login_btn_forgot_password)
            {
                StartActivity(new Android.Content.Intent(this, typeof(ForgotPassword)));
                Finish();
            }
            else if (v.Id == Resource.Id.login_btn_sign_up)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUp)));
                Finish();
            }
            else if (v.Id == Resource.Id.login_btn_login)
            {
                if (IsValidEmail(input_email.Text) && input_password.Text.Length > 0)
                {
                    LoginUser(input_email.Text, input_password.Text);
                }
                else
                {
                    Toast.MakeText(this, "Pogrešan email ili lozinka!", ToastLength.Short).Show();
                }
            }
        }

        private void LoginUser(string email, string password)
        {
            var connection = CrossConnectivity.Current.IsConnected;
            if (connection)
            {
                auth.SignInWithEmailAndPassword(email, password)
                    .AddOnCompleteListener(this);

                progress_bar.Visibility = ViewStates.Visible;
                activity_main.Visibility = ViewStates.Invisible;
            }
            else
            {
                Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
            }
        }


        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                StartActivity(new Android.Content.Intent(this, typeof(MainActivity)));
                Finish();
            }
            else
            {
                progress_bar.Visibility = ViewStates.Invisible;
                activity_main.Visibility = ViewStates.Visible;
                Toast.MakeText(this, "Dogodila se greška. Pokušajte ponovo!", ToastLength.Long).Show();
            }
        }
    }
}