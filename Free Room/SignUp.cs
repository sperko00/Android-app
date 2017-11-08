using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using static Android.Views.View;
using Android.Gms.Tasks;
using Firebase.Auth;
using System.Net.Mail;
using Plugin.Connectivity;

namespace Free_Room
{
    [Activity(Label = "SignUp", Theme = "@style/LoginTheme")]
    public class SignUp : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        LinearLayout signup_act;
        Button btnSignup;
        TextView btnLogin, btnForgotPass;
        EditText input_email, input_password;
        FirebaseAuth auth;
        ImageView logo_img;
        private ProgressBar progress_bar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);

            //InitFirebase
            auth = FirebaseAuth.GetInstance(Login.app);

            //View
            btnSignup = FindViewById<Button>(Resource.Id.signup_btn_register);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);
            btnForgotPass = FindViewById<TextView>(Resource.Id.signup_btn_forgot_password);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            progress_bar = FindViewById<ProgressBar>(Resource.Id.progress_bar_signup);
            signup_act = FindViewById<LinearLayout>(Resource.Id.activity_sign_up1);
            logo_img = FindViewById<ImageView>(Resource.Id.icon);
            btnLogin.SetOnClickListener(this);
            btnForgotPass.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);
        }
        private static bool isValidEmail(string EmailToCheck)
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
        public void OnClick(View v)
        {
            int e = 0, p = 0;
            if (v.Id == Resource.Id.signup_btn_login)
            {
                StartActivity(new Intent(this, typeof(Login)));
                Finish();
            }
            else if (v.Id == Resource.Id.signup_btn_forgot_password)
            {
                StartActivity(new Intent(this, typeof(ForgotPassword)));
                Finish();
            }
            else if (v.Id == Resource.Id.signup_btn_register)
            {
                if (isValidEmail(input_email.Text))
                {
                    e = 1;
                }
                else
                {
                    e = 0;
                    Toast.MakeText(this, "Email nije valjan!", ToastLength.Long).Show();
                }
                if (input_password.Text.Length >= 6)
                {
                    p = 1;
                }
                else
                {
                    p = 0;
                    Toast.MakeText(this, "Lozinka mora sadržavati najmanje 6 znakova!", ToastLength.Long).Show();
                }
                if (e == 1 && p == 1)
                {
                    SignUpUser(input_email.Text, input_password.Text);
                }


            }
        }

        private void SignUpUser(string email, string password)
        {
            var connection = CrossConnectivity.Current.IsConnected;
            if (connection)
            {
                auth.CreateUserWithEmailAndPassword(email, password)
                .AddOnCompleteListener(this, this);
                signup_act.Visibility = ViewStates.Invisible;
                progress_bar.Visibility = ViewStates.Visible;
            }
            else
            {
                Toast.MakeText(this, "Niste povezani na Internet!", ToastLength.Short).Show();
            }

        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Toast.MakeText(this, "Registracija uspješna!", ToastLength.Long).Show();
                Finish();
                StartActivity(new Intent(this, typeof(Login)));

            }
            else
            {
                progress_bar.Visibility = ViewStates.Invisible;
                signup_act.Visibility = ViewStates.Visible;
                Toast.MakeText(this, "Dogodila se greška. Pokušajte ponovo!", ToastLength.Long).Show();

            }

        }
    }
}