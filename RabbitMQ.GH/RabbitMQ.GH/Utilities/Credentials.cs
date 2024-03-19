using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using RabbitMQ.GH.Properties;
using Rhino.Geometry;

namespace RabbitMQ.GH.Utilities
{
    /// <summary>
    /// Represents a class to handle authentication credentials within grasshopper.
    /// Inherits from the GH_component class.
    /// </summary>
    public class Credentials : GH_Component
    {

        #region Constructor

        /// <summary>
        /// Default constructor. Invokes the base class constructor.
        /// </summary>
        public Credentials()
          : base("Credentials", "Cred",
              "Set the credentials for the RabbitMQ connection (only once per rhino session)",
              "RabbitMQ", "Utilities")
        {
        }
        
        #endregion Constructor

        /// <summary>
        /// Represents the exposure level of the component.
        /// </summary>
        /// /// <value>
        /// The value of 2 sets the Consumer component to be displayed in the first section of the Grasshopper toolbar.
        /// </value>
        public override GH_Exposure Exposure => (GH_Exposure)2;

        #region Methods

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (Rhino.RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_USER"))
            {
                if (Rhino.RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"] != null)
                {
                    LogoutForm logform = new LogoutForm();
                    logform.ShowDialog();

                    bool logout = logform.Log_out;
                    if (logout)
                    {
                        Rhino.RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"] = null;
                        Rhino.RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_PASS"] = null;
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You are now logeed out of the RabbitMQ broker");
                    }
                    else
                    {
                        return;
                    }

                }
            }

            // Declares and shows a window for the user to input the login credentials.
            LoginForm form = new LoginForm();
            form.ShowDialog();

            /// Reads the credentials from form and stores the user´s username and password
            /// into the corresponding variables.
            string username = form.Username;
            string password = form.Password;

            /// Adds the username to RuntimeData, a property accessible from the document´s scope. 
            /// This makes the credentials available outside the  input component.
            Rhino.RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"] = username;
            Rhino.RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_PASS"] = password;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.CER;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3E7DE11C-2D30-4AFE-AF85-9DC2BA4D50EB"); }
        }

        #endregion methods
    }

    /// <summary>
    /// Represents a user interface to input credentials for authentication.
    /// Inherits from the Form class.
    /// </summary>
    public class LoginForm : Form
    {
        // Public properties to access the user input data from the component
        public string Username { get; private set; }
        public string Password { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LoginForm()
        {
            // Sets up the form properties
            Text = "RabbitMQ Credentials";
            Width = 400;
            Height = 200;
            // Form initialization and controls setup as before...

            // Loads the logo image from file
            //string logoFilePath = DesFab.GH.Properties.Resources.DesFab1; // Replace with the path to your logo image
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(logoFilePath);
            System.Drawing.Bitmap bmp = Resources.BASIC;
            this.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());

            // Set the logo as the form's icon
            //this.Icon = new System.Drawing.Icon(logoFilePath);

            // Creates the controls for username and password input
            Label usernameLabel = new Label()
            {
                Text = "Username:",
                Left = 20,
                Top = 20,
                Width = 100
            };

            TextBox usernameTextBox = new TextBox()
            {
                Left = 120,
                Top = 20,
                Width = 200
            };

            Label passwordLabel = new Label()
            {
                Text = "Password:",
                Left = 20,
                Top = 50,
                Width = 100
            };

            TextBox passwordTextBox = new TextBox()
            {
                Left = 120,
                Top = 50,
                Width = 200,
                PasswordChar = '*' // Show asterisks for password input
            };

            Button button = new Button()
            {
                Text = "OK",
                Left = 120,
                Top = 80,
                Width = 100
            };

            // Add the controls to the form
            Controls.Add(usernameLabel);
            Controls.Add(usernameTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(passwordTextBox);
            Controls.Add(button);

            // Attach the button's click event handler
            button.Click += (sender, e) =>
            {
                // Access the user's input data here
                Username = usernameTextBox.Text;
                Password = passwordTextBox.Text;

                // Closes the form with a result indicating the "OK" button was clicked
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }

    /// <summary>
    /// Represents a user interface to logout from the RabbitMQ server.
    /// Inherits from the Form class.
    /// </summary>
    public class LogoutForm : Form
    {
        // Public properties to access the user input data from the component
        public bool Log_out { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LogoutForm()
        {

            // Sets up the form properties
            Text = "RabbitMQ Credentials";
            Width = 400;
            Height = 200;
            // Form initialization and controls setup as before...

            // Loads the logo image from file
            //string logoFilePath = DesFab.GH.Properties.Resources.DesFab1; // Replace with the path to your logo image
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(logoFilePath);
            System.Drawing.Bitmap bmp = Resources.BASIC;
            this.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());

            // Sets the logo as the form's icon
            //this.Icon = new System.Drawing.Icon(logoFilePath);

            // Creates the controls for username and password input
            Label usernameLabel = new Label()
            {
                Text = "You are logged in as " + Rhino.RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"],
                Left = 20,
                Top = 20,
                Width = 220
            };


            Button logout = new Button()
            {
                Text = "Logout",
                Left = 120,
                Top = 80,
                Width = 100
            };

            Button confirm = new Button()
            {
                Text = "Continue",
                Left = 220,
                Top = 80,
                Width = 100
            };

            // Adds the controls to the form
            Controls.Add(usernameLabel);
            Controls.Add(logout);
            Controls.Add(confirm);

            // Attaches the logout button's click event handler
            logout.Click += (sender, e) =>
            {
                // Access the user's input data here
                Log_out = true;
                // Close the form with a result indicating the "OK" button was clicked
                DialogResult = DialogResult.OK;
                Close();
            };

            // Attaches the confirmation button's click event handler
            confirm.Click += (sender, e) =>
            {
                // Access the user's input data here
                Log_out = false;
                // Close the form with a result indicating the "OK" button was clicked
                DialogResult = DialogResult.OK;
                Close();
            };


        }
    }

}