using System;
using System.Collections.Generic;
using Terminal.Gui;
using System.Security.Cryptography;
using System.Text;
using progbase3;
public class Authentification
{
    public class RegistrationAndAuthorization : Dialog
    {
        private UserRepository repository;
        private TextField fullnameInput;
        private TextField usernameInput;
        private TextField passwordInput;
        private User signedInUser;
        public bool signedIn;
        public RegistrationAndAuthorization(UserRepository repository)
        {
            this.repository = repository;
            this.Title = "Hello";
            int rightColumnX = 40;

            Label fullNameLbl = new Label(2, 2, "Fullname (only to sign up):");
            fullnameInput = new TextField()
            {
                X = rightColumnX,
                Y = Pos.Top(fullNameLbl),
                Width = 40,
            };
            this.Add(fullNameLbl, fullnameInput);

            Label usernameLbl = new Label(2, 4, "Username:");
            usernameInput = new TextField()
            {
                X = rightColumnX,
                Y = Pos.Top(usernameLbl),
                Width = 40,
            };
            this.Add(usernameLbl, usernameInput);

            Label passwordLbl = new Label(2, 6, "Password:");
            passwordInput = new TextField()
            {
                X = rightColumnX,
                Y = Pos.Top(passwordLbl),
                Width = 40,
                Secret = true,
            };
            this.Add(passwordLbl, passwordInput);

            Button signInBtn = new Button("Sign in")
            {
                X = rightColumnX,
                Y = Pos.Top(passwordLbl) + 5,
            };

            Button signUpBtn = new Button("Sign up")
            {
                X = rightColumnX + 15,
                Y = Pos.Top(passwordLbl) + 5,
            };

            this.Add(signInBtn, signUpBtn);

            signUpBtn.Clicked += OnSignUp;
            signInBtn.Clicked += OnSignIn;



        }
        private void OnSignIn()
        {
            if (usernameInput.Text == "" || passwordInput.Text == "")
            {
                int response = MessageBox.ErrorQuery("ERROR", "Username and/or password fields are empty.", "OK");
                if (response == 0)
                {
                    return;
                }
            }
            if (!repository.UserExists(usernameInput.Text.ToString()))
            {
                int response = MessageBox.ErrorQuery("ERROR", "User with this username doesn`t exist.", "OK");
                if (response == 0)
                {
                    return;
                }
            }
            User newUser = repository.GetByUsername(usernameInput.Text.ToString());
            string enteredPasswordHash = PasswordHashing.GetPasswordHash(passwordInput.Text.ToString());
            if (enteredPasswordHash != newUser.password)
            {
                int response = MessageBox.ErrorQuery("ERROR", "Incorrect password.", "OK");
                if (response == 0)
                {
                    return;
                }
            }
            this.signedInUser = newUser;
            signedIn = true;
            Application.RequestStop();

        }
        private void OnSignUp()
        {
            if (fullnameInput.Text == "" || usernameInput.Text == "" || passwordInput.Text == "")
            {
                int response = MessageBox.ErrorQuery("ERROR", "Some fields are empty.", "OK");
                if (response == 0)
                {
                    return;
                }
            }
            if (repository.UserExists(usernameInput.Text.ToString()))
            {
                int response = MessageBox.ErrorQuery("ERROR", "User with this username already exists.", "OK");
                if (response == 0)
                {
                    return;
                }
            }
            string passwordHash = PasswordHashing.GetPasswordHash(passwordInput.Text.ToString());
            User userToSignUp = new User(fullnameInput.Text.ToString(), usernameInput.Text.ToString(), passwordHash);
            repository.Insert(userToSignUp);
            int responseOk = MessageBox.Query("Signed up", "The registration was successful, now please sign in", "OK");
            if (responseOk == 0)
            {
                return;
            }


        }
        public User GetUser()
        {
            return signedInUser;
        }
    }
    public static class PasswordHashing
    {
        public static string GetPasswordHash(string source)
        {
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, source);
            return hash;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}
