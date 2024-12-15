open System
open System.Windows.Forms
open System.Drawing
open LibraryManagement  // Ensure you have the LibraryManagement module

// Define a type to store the current user type (Admin or User)
type UserType = Admin | User

let showLoginForm () =
    let loginForm = new Form(Text = "Login Page", Width = 500, Height = 400)
    loginForm.StartPosition <- FormStartPosition.CenterScreen
    loginForm.BackColor <- Color.White

    // Label for "Enter your data"
    let titleLabel = new Label(Text = "Enter your data", Top = 20, Left = 30, Width = 440, Height = 40)
    titleLabel.Font <- new Font("Arial", 18.0f, FontStyle.Bold)
    titleLabel.ForeColor <- Color.MidnightBlue
    titleLabel.TextAlign <- ContentAlignment.MiddleCenter

    // Username and Password Inputs
    let labelTop = 80
    let usernameLabel = new Label(Text = "Username:", Top = labelTop, Left = 30, Width = 120, Height = 30)
    let usernameInput = new TextBox(Top = labelTop + 30, Left = 30, Width = 420, Height = 80)
    let passwordLabel = new Label(Text = "Password:", Top = labelTop + 90, Left = 30, Width = 120, Height = 30)
    let passwordInput = new TextBox(Top = labelTop + 120, Left = 30, Width = 420, Height = 40)
    
    // Configure inputs
    passwordInput.PasswordChar <- '*'
    usernameInput.Font <- new Font("Arial", 12.0f)
    passwordInput.Font <- new Font("Arial", 12.0f)

    // Submit Button
    let submitButton = new Button(Text = "Submit", Top = labelTop + 180, Left = 160, Width = 140, Height = 50)
    submitButton.Font <- new Font("Arial", 14.0f, FontStyle.Bold)
    submitButton.BackColor <- Color.FromArgb(100, 193, 150)
    submitButton.ForeColor <- Color.White
    submitButton.FlatStyle <- FlatStyle.Flat
    submitButton.FlatAppearance.BorderSize <- 0
    submitButton.FlatAppearance.MouseOverBackColor <- Color.FromArgb(28, 161, 166)

    // Status label for login attempt feedback
    let loginStatusLabel = new Label(Text = "", Top = labelTop + 250, Left = 30, Width = 440, Height = 30)
    loginStatusLabel.ForeColor <- Color.Red
    loginStatusLabel.Font <- new Font("Arial", 12.0f, FontStyle.Italic)
    loginStatusLabel.TextAlign <- ContentAlignment.MiddleCenter

    let userType = ref None
    let loginSuccessful = ref false

    // Handling submit button click event
    submitButton.Click.Add(fun _ -> 
        match usernameInput.Text, passwordInput.Text with
        | "admin", "123" -> 
            userType := Some Admin
            loginSuccessful := true
            loginForm.Close()
        | "user", "123" -> 
            userType := Some User
            loginSuccessful := true
            loginForm.Close()
        | _ -> 
            loginStatusLabel.Text <- "Invalid credentials, please try again!"
            loginStatusLabel.ForeColor <- Color.Red
    )

    // Add controls to the form
    loginForm.Controls.AddRange([| titleLabel; usernameLabel; usernameInput; passwordLabel; passwordInput; submitButton; loginStatusLabel |])

    // Handle form closing event
    loginForm.FormClosing.Add(fun _ -> 
        if not !loginSuccessful then
            MessageBox.Show("Login failed, exiting application.") |> ignore
            Environment.Exit(0) // Exit application if login is unsuccessful
    )

    // Show the login form as a dialog
    loginForm.ShowDialog()

    // Return the userType after the login form closes
    !userType


    // Admin Page with enhanced design and layout
let showAdminPage () =
    let form = new Form(Text = "Admin - Library Management", Width = 1000, Height = 600)
    form.StartPosition <- FormStartPosition.CenterScreen
    form.BackColor <- System.Drawing.Color.LightSkyBlue

    let tabControl = new TabControl()
    tabControl.Width <- 900
    tabControl.Height <- 500
    tabControl.Top <- 30
    tabControl.Left <- 50

    // Add Book Tab
    let addBookTab = new TabPage("Add Book")
    addBookTab.BackColor <- Color.LightGoldenrodYellow

    let titleLabel = new Label(Text = "Title:", Top = 20, Left = 30, Width = 100)
    titleLabel.Font <- new Font("Arial", 12.0f)
    let authorLabel = new Label(Text = "Author:", Top = 60, Left = 30, Width = 100)
    authorLabel.Font <- new Font("Arial", 12.0f)
    let genreLabel = new Label(Text = "Genre:", Top = 100, Left = 30, Width = 100)
    genreLabel.Font <- new Font("Arial", 12.0f)

    let titleInput = new TextBox(Top = 20, Left = 150, Width = 250)
    titleInput.Font <- new Font("Arial", 12.0f)
    let authorInput = new TextBox(Top = 60, Left = 150, Width = 250)
    authorInput.Font <- new Font("Arial", 12.0f)
    let genreInput = new TextBox(Top = 100, Left = 150, Width = 250)
    genreInput.Font <- new Font("Arial", 12.0f)

    let addBookButton = new Button(Text = "Add Book", Top = 140, Left = 150, Width = 250, Height = 40)
    addBookButton.BackColor <- Color.CadetBlue
    addBookButton.Font <- new Font("Arial", 12.0f, FontStyle.Bold)
    addBookButton.Click.Add(fun _ -> 
        Library.addBook titleInput.Text authorInput.Text genreInput.Text
        MessageBox.Show($"Book '{titleInput.Text}' added.") |> ignore
    )

    addBookTab.Controls.AddRange([| titleLabel; titleInput; authorLabel; authorInput; genreLabel; genreInput; addBookButton |])

    // View Books Tab
    let viewBooksTab = new TabPage("View Books")
    viewBooksTab.BackColor <- Color.LightGoldenrodYellow

    let availableBooksButton = new Button(Text = "Available Books", Top = 20, Left = 20, Width = 150, Height = 40)
    availableBooksButton.BackColor <- Color.CadetBlue
    availableBooksButton.Font <- new Font("Arial", 12.0f)
    let borrowedBooksButton = new Button(Text = "Borrowed Books", Top = 20, Left = 200, Width = 150, Height = 40)
    borrowedBooksButton.BackColor <- Color.CadetBlue
    borrowedBooksButton.Font <- new Font("Arial", 12.0f)
    let booksListBox = new ListBox(Top = 70, Left = 20, Width = 850, Height = 300)

    availableBooksButton.Click.Add(fun _ -> 
        let availableBooks = 
            Library.getBooks()
            |> Seq.filter (fun book -> not book.IsBorrowed)
            |> Seq.map (fun book -> $"{book.Title} by {book.Author}")
            |> String.concat "\n"
        booksListBox.Items.Clear()
        booksListBox.Items.AddRange(availableBooks.Split('\n') |> Array.map (fun book -> box book))
    )

    borrowedBooksButton.Click.Add(fun _ -> 
        let borrowedBooks = 
            Library.getBooks()
            |> Seq.filter (fun book -> book.IsBorrowed)
            |> Seq.map (fun book -> $"{book.Title} by {book.Author}")
            |> String.concat "\n"
        booksListBox.Items.Clear()
        booksListBox.Items.AddRange(borrowedBooks.Split('\n') |> Array.map (fun book -> box book))
    )

    viewBooksTab.Controls.AddRange([| availableBooksButton; borrowedBooksButton; booksListBox |])

    // Add Tabs to TabControl
    tabControl.TabPages.Add(addBookTab)
    tabControl.TabPages.Add(viewBooksTab)

    form.Controls.Add(tabControl)
    form.ShowDialog()


// User Page with refined design and improved search functionality
let showUserPage () =
    let Userform = new Form(Text = "User - Library Management", Width = 800, Height = 500)
    Userform.StartPosition <- FormStartPosition.CenterScreen
    Userform.BackColor <- System.Drawing.Color.MistyRose

    // Create TabControl for organized layout
    let tabControl = new TabControl()
    tabControl.Width <- 760
    tabControl.Height <- 400
    tabControl.Top <- 30
    tabControl.Left <- 20

    // Create "Borrow/Return Books" Tab with a cleaner layout
    let borrowReturnTab = new TabPage("Borrow/Return Books")
    borrowReturnTab.BackColor <- Color.Honeydew

    let titleLabel = new Label(Text = "Title:", Top = 20, Left = 20, Width = 100)
    let titleInput = new TextBox(Top = 20, Left = 120, Width = 200)

    let borrowButton = new Button(Text = "Borrow Book", Top = 60, Left = 120, Width = 150, Height = 40)
    borrowButton.BackColor <- Color.LightCoral  // Changed to LightCoral
    let returnButton = new Button(Text = "Return Book", Top = 60, Left = 300, Width = 150, Height = 40)
    returnButton.BackColor <- Color.LightCoral  // Changed to LightCoral

    let borrowReturnStatusLabel = new Label(Top = 120, Left = 20, Width = 740, Height = 30)
    borrowReturnStatusLabel.ForeColor <- Color.Red
    borrowReturnStatusLabel.TextAlign <- ContentAlignment.MiddleCenter

    borrowButton.Click.Add(fun _ -> 
        if Library.borrowBook titleInput.Text then
            borrowReturnStatusLabel.Text <- $"Book '{titleInput.Text}' borrowed."
        else
            borrowReturnStatusLabel.Text <- "Borrowing failed."
    )

    returnButton.Click.Add(fun _ -> 
        if Library.returnBook titleInput.Text then
            borrowReturnStatusLabel.Text <- $"Book '{titleInput.Text}' returned."
        else
            borrowReturnStatusLabel.Text <- "Returning failed."
    )

    borrowReturnTab.Controls.AddRange([| titleLabel; titleInput; borrowButton; returnButton; borrowReturnStatusLabel |])

    // Create "Available Books" Tab with more attractive design
    let availableBooksTab = new TabPage("Available Books")
    availableBooksTab.BackColor <- Color.Honeydew

    let availableBooksButton = new Button(Text = "Show Available Books", Top = 20, Left = 20, Width = 200, Height = 40)
    availableBooksButton.BackColor <- Color.LightCoral  // Changed to LightCoral
    let booksListBox = new ListBox(Top = 70, Left = 20, Width = 700, Height = 250)

    availableBooksButton.Click.Add(fun _ -> 
        let availableBooks = 
            Library.getBooks()
            |> Seq.filter (fun book -> not book.IsBorrowed)
            |> Seq.map (fun book -> $"{book.Title} by {book.Author}")
            |> String.concat "\n"
        booksListBox.Items.Clear()
        booksListBox.Items.AddRange(availableBooks.Split('\n') |> Array.map (fun book -> box book))
    )

    availableBooksTab.Controls.AddRange([| availableBooksButton; booksListBox |])

    // Add Tabs to TabControl
    tabControl.TabPages.Add(borrowReturnTab)
    tabControl.TabPages.Add(availableBooksTab)

    Userform.Controls.Add(tabControl)
    Userform.ShowDialog()

let showHomePage () =
    let form = new Form(Text = "Library Management System", Width = 800, Height = 500)
    form.StartPosition <- FormStartPosition.CenterScreen
    form.BackColor <- Color.White

    // Panel for top section with background color and gradient
    let topPanel = new Panel(Dock = DockStyle.Top, Height = 120)
    let gradientBrush = new Drawing2D.LinearGradientBrush(topPanel.ClientRectangle, Color.FromArgb(0, 120, 215), Color.FromArgb(0, 255, 255), Drawing2D.LinearGradientMode.Vertical)
    let graphics = topPanel.CreateGraphics()
    graphics.FillRectangle(gradientBrush, topPanel.ClientRectangle)
    form.Controls.Add(topPanel)

    // Welcome message with background color covering the full width of the form
    let welcomeLabel = new Label(Text = "Welcome to the Library Management System", Top = 0, Left = 0, Width = form.Width, Height = 70)
    welcomeLabel.Font <- new Font("Arial", 24.0f, FontStyle.Bold)
    welcomeLabel.ForeColor <- Color.White
    welcomeLabel.TextAlign <- ContentAlignment.MiddleCenter
    welcomeLabel.BackColor <- Color.Gray // Background color for the label
    topPanel.Controls.Add(welcomeLabel)

    // Short info about the library system (centered in the page)
    let infoLabel = new Label(Text = "Manage Books, Borrow or Return Them, View Available books, and more.", Top = 120, Left = 30, Width = 720, Height = 30)
    infoLabel.Font <- new Font("Arial", 16.0f)
    infoLabel.ForeColor <- Color.Gray
    infoLabel.TextAlign <- ContentAlignment.MiddleCenter
    form.Controls.Add(infoLabel)

    let infoLabel2 = new Label(Text = "You Can Login Now And Enjoy Reading Books!", Top = 150, Left = 30, Width = 720, Height = 30)
    infoLabel2.Font <- new Font("Arial", 16.0f)
    infoLabel2.ForeColor <- Color.Gray
    infoLabel2.TextAlign <- ContentAlignment.MiddleCenter
    form.Controls.Add(infoLabel2)

    // Login Button with hover effect and icon (centered in the page)
    let loginButton = new Button(Text = "Login", Top = 200, Left = (form.Width / 2) - 100, Width = 200, Height = 50)
    loginButton.Font <- new Font("Arial", 14.0f, FontStyle.Bold)
    loginButton.BackColor <- Color.CadetBlue
    loginButton.ForeColor <- Color.White
    loginButton.FlatStyle <- FlatStyle.Flat
    loginButton.FlatAppearance.BorderSize <- 0
    loginButton.FlatAppearance.MouseOverBackColor <- Color.FromArgb(100, 193, 150)
    loginButton.Click.Add(fun _ -> 
        let userType = showLoginForm()  // Display login form and get user type
        match userType with
        | Some Admin -> showAdminPage() |> ignore  // Admin page with full functionality
        | Some User -> showUserPage() |> ignore // User page with limited functionality
        | None -> ()  // Do nothing if the form is closed without login
    )

    // Add buttons and labels to the form
    form.Controls.Add(loginButton)
    form.ShowDialog()

// Main Application Flow
[<STAThread>]
do
    showHomePage()  // Display the home page


