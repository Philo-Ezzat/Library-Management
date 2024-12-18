﻿open System
open System.Windows.Forms
open System.Drawing
open LibraryManagement  // Ensure you have the LibraryManagement module
open System.IO
open Newtonsoft.Json
open System.Security.Cryptography
open System.Text
open LibraryManagement.Library

// Define a type to store the current user type (Admin or User)
type UserType = Admin | User
type User = {
    Username: string
    Password: string
}
// hashing the password
let hashPassword (password: string) =
    using (SHA256.Create()) (fun sha256 ->
        let bytes = Encoding.UTF8.GetBytes(password)
        let hash = sha256.ComputeHash(bytes)
        BitConverter.ToString(hash).Replace("-", "").ToLower()
    )
let usersFilePath = "users.json"
let saveUser (user: User) =
    let users = 
        if File.Exists(usersFilePath) then
            let json = File.ReadAllText(usersFilePath)
            JsonConvert.DeserializeObject<User list>(json)
        else
            []
    let updatedUsers = user :: users
    let updatedJson = JsonConvert.SerializeObject(updatedUsers, Formatting.Indented)
    File.WriteAllText(usersFilePath, updatedJson)

let showSignupForm () =
    let signupForm = new Form(Text = "Signup Page", Width = 500, Height = 400)
    signupForm.StartPosition <- FormStartPosition.CenterScreen
    signupForm.BackColor <- Color.White

    // Label for "Create a new account"
    let titleLabel = new Label(Text = "Create a new account", Top = 20, Left = 30, Width = 440, Height = 40)
    titleLabel.Font <- new Font("Arial", 18.0f, FontStyle.Bold)
    titleLabel.ForeColor <- Color.MidnightBlue
    titleLabel.TextAlign <- ContentAlignment.MiddleCenter

    // Username and Password Inputs
    let labelTop = 80
    let usernameLabel = new Label(Text = "Username:", Top = labelTop, Left = 30, Width = 120, Height = 30)
    let usernameInput = new TextBox(Top = labelTop + 30, Left = 30, Width = 420, Height = 40)
    let passwordLabel = new Label(Text = "Password:", Top = labelTop + 90, Left = 30, Width = 120, Height = 30)
    let passwordInput = new TextBox(Top = labelTop + 120, Left = 30, Width = 420, Height = 40)
    
    // Configure inputs
    passwordInput.PasswordChar <- '*'
    usernameInput.Font <- new Font("Arial", 12.0f)
    passwordInput.Font <- new Font("Arial", 12.0f)

    // Submit Button
    let submitButton = new Button(Text = "Signup", Top = labelTop + 180, Left = 160, Width = 140, Height = 50)
    submitButton.Font <- new Font("Arial", 14.0f, FontStyle.Bold)
    submitButton.BackColor <- Color.FromArgb(100, 193, 150)
    submitButton.ForeColor <- Color.White
    submitButton.FlatStyle <- FlatStyle.Flat
    submitButton.FlatAppearance.BorderSize <- 0
    submitButton.FlatAppearance.MouseOverBackColor <- Color.FromArgb(28, 161, 166)

    // Status label for signup attempt feedback
    let signupStatusLabel = new Label(Text = "", Top = labelTop + 250, Left = 30, Width = 440, Height = 30)
    signupStatusLabel.ForeColor <- Color.Red
    signupStatusLabel.Font <- new Font("Arial", 12.0f, FontStyle.Italic)
    signupStatusLabel.TextAlign <- ContentAlignment.MiddleCenter

    // Handling submit button click event
    submitButton.Click.Add(fun _ -> 
        if usernameInput.Text <> "" && passwordInput.Text <> "" then
            let hashedPassword = hashPassword passwordInput.Text
            let newUser = { Username = usernameInput.Text; Password = hashedPassword }
            Console.WriteLine($"New User: {newUser}")
            saveUser newUser
            signupStatusLabel.Text <- "Signup successful!"
            signupStatusLabel.ForeColor <- Color.Green
            // Clear the input fields after successful signup and close the form
            usernameInput.Text <- ""
            passwordInput.Text <- ""
            let timer = new Timer()
            timer.Interval <- 500 // 2 seconds
            timer.Tick.Add(fun _ -> 
                timer.Stop()
                signupForm.Invoke(new Action(fun () -> signupForm.Close())) // Close the form on the UI thread
            )
            timer.Start()
        else
            signupStatusLabel.Text <- "Please fill in all fields."
            signupStatusLabel.ForeColor <- Color.Red
    )

    // Add controls to the form
    signupForm.Controls.AddRange([| titleLabel; usernameLabel; usernameInput; passwordLabel; passwordInput; submitButton; signupStatusLabel |])

    // Show the signup form as a dialog
    signupForm.ShowDialog()

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
    let userName = ref ""
    // Handling submit button click event
    submitButton.Click.Add(fun _ -> 
        let hashedPassword = hashPassword passwordInput.Text
        let usersFilePath = "users.json"
        if File.Exists(usersFilePath) then
            let json = File.ReadAllText(usersFilePath)
            let users = JsonConvert.DeserializeObject<User list>(json)
            match users |> List.tryFind (fun user -> user.Username = usernameInput.Text && user.Password = hashedPassword) with
            | Some user when user.Username = "admin" -> 
                userType := Some Admin
                loginSuccessful := true
                userName := user.Username
                loginForm.Close()
            | Some user -> 
                userType := Some User
                loginSuccessful := true
                userName := user.Username
                loginForm.Close()
            | None -> 
                loginStatusLabel.Text <- "Invalid credentials, please try again!"
                loginStatusLabel.ForeColor <- Color.Red
        else
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
    (!userType, !userName)


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
    let priceLabel = new Label(Text = "Price:", Top = 140, Left = 30, Width = 100)
    priceLabel.Font <- new Font("Arial", 12.0f)

    let titleInput = new TextBox(Top = 20, Left = 150, Width = 250)
    titleInput.Font <- new Font("Arial", 12.0f)
    let authorInput = new TextBox(Top = 60, Left = 150, Width = 250)
    authorInput.Font <- new Font("Arial", 12.0f)
    let genreInput = new TextBox(Top = 100, Left = 150, Width = 250)
    genreInput.Font <- new Font("Arial", 12.0f)
    let priceInput = new TextBox(Top = 140, Left = 150, Width = 250)
    priceInput.Font <- new Font("Arial", 12.0f)
    let addBookButton = new Button(Text = "Add Book", Top = 180, Left = 150, Width = 250, Height = 40)
    addBookButton.BackColor <- Color.CadetBlue
    addBookButton.Font <- new Font("Arial", 12.0f, FontStyle.Bold)
    addBookButton.Click.Add(fun _ -> 
        Library.addBook titleInput.Text authorInput.Text genreInput.Text priceInput.Text
        MessageBox.Show($"Book '{titleInput.Text}' added.") |> ignore
    )

    addBookTab.Controls.AddRange([| titleLabel; titleInput; authorLabel; authorInput; genreLabel; genreInput;priceLabel; priceInput; addBookButton |])

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

    let  loadRevenueFromFile () =
        let revenueFilePath = "revenue.json"
        if File.Exists(revenueFilePath) then
            let json = File.ReadAllText(revenueFilePath)
            match Decimal.TryParse(json) with
            | (true, revenue) -> revenue
            | _ -> 0M
        else
            0M

    let viewRevenueTab = new TabPage("View Total Revenue")
    viewRevenueTab.BackColor <- Color.LightGoldenrodYellow

    let revenueLabel = new Label(Text = "Total Revenue: $0.00", Top = 20, Left = 30, Width = 200)
    revenueLabel.Font <- new Font("Arial", 12.0f)

    // Retrieve the total revenue and update the label text
    let totalRevenue = loadRevenueFromFile()
    revenueLabel.Text <- sprintf "Total Revenue: $%M" totalRevenue

    viewRevenueTab.Controls.AddRange([| revenueLabel |])

    tabControl.TabPages.Add(addBookTab)
    tabControl.TabPages.Add(viewBooksTab)
    tabControl.TabPages.Add(viewRevenueTab)
    form.Controls.Add(tabControl)
    form.ShowDialog()


// User Page with refined design and improved search functionality
let showUserPage (userName: string) =
    let Userform = new Form(Text = $"User - Library Management ({userName})", Width = 800, Height = 500)
    Userform.StartPosition <- FormStartPosition.CenterScreen
    Userform.BackColor <- System.Drawing.Color.MistyRose

    // Create TabControl for organized layout
    let tabControl = new TabControl()
    tabControl.Width <- 760
    tabControl.Height <- 400
    tabControl.Top <- 30
    tabControl.Left <- 20

    // Create "Borrow/Return Books" Tab with a cleaner layout
// Create "Borrow/Return Books" Tab with a cleaner layout
    let borrowReturnTab = new TabPage("Books")
    borrowReturnTab.BackColor <- Color.Honeydew

    // Title label and input for the search bar
    let searchLabel = new Label(Text = "Search Book by Title:", Top = 20, Left = 20, Width = 150)
    let searchInput = new TextBox(Top = 20, Left = 170, Width = 200)

    // Search button to trigger the search functionality
    let searchButton = new Button(Text = "Search", Top = 20, Left = 450, Width = 100, Height = 20)
    searchButton.BackColor <- Color.LightCoral


    // Panel to hold the book list and dynamically update it
    let booksPanel = new Panel(Top = 50, Left = 20, Width = 740, Height = 300)
    booksPanel.AutoScroll <- true

    // Function to display books in the panel
    let displayBooks (books: seq<Book>) =
        // Clear existing controls in booksPanel
        booksPanel.Controls.Clear()

        // Filter the books to show only available ones
        let availableBooks = books
    
        // Dynamically add each available book as a label to the panel
        for book in availableBooks do
            let bookLabel = new Label(Text = $"""{book.Title} by {book.Author} - {book.Genre} - {book.Price} - {(if book.IsBorrowed then "Borrowed" else "Available")}""")
            bookLabel.Width <- 700
            bookLabel.Top <- 30 * booksPanel.Controls.Count + 10 // Add spacing between books
            booksPanel.Controls.Add(bookLabel)

    // Initially display all books
    displayBooks (getBooks())

    // Search functionality
    searchButton.Click.Add(fun _ ->
        let searchTerm = searchInput.Text
        if String.IsNullOrEmpty(searchTerm) then
            // If no search term, show all books
            displayBooks (getBooks())
        else
            // If there's a search term, show matching books
            let searchResults = searchBooksByName searchTerm
            displayBooks searchResults
    )


    // Borrow button functionality




    let titleLabel = new Label(Text = "Title:", Top = 20, Left = 20, Width = 100)
    let titleInput = new TextBox(Top = 20, Left = 120, Width = 200)

    borrowReturnTab.Controls.AddRange([| 
        searchLabel; 
        searchInput; 
        searchButton; 
        titleLabel; 
        titleInput; 
        booksPanel 
    |])



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
            |> Seq.map (fun book -> $"Title:\t{book.Title}\t\tAuthor:\t{book.Author}\t\tBorrowPrice:\t{book.Price}\t\tGenre:\t{book.Genre}")
            |> String.concat "\n"
        booksListBox.Items.Clear()
        booksListBox.Items.AddRange(availableBooks.Split('\n') |> Array.map (fun book -> box book))
    )

    let borrowButton = new Button(Text = "Borrow Book", Top = 330, Left = 20, Width = 200, Height = 40)
    borrowButton.BackColor <- Color.LightCoral  // Changed to LightCoral

    borrowButton.Click.Add(fun _ -> 
        let selectedBook = booksListBox.SelectedItem

        // Ensure the selectedItem is a string
        if selectedBook <> null then
            let selectedBookString = selectedBook.ToString() // Cast the selected item to string

            // Extract book title from the formatted string
            let bookTitle = 
                selectedBookString.Split([| "Title:" |], StringSplitOptions.None).[1]
                |> fun part -> part.Split([| "Author:" |], StringSplitOptions.None).[0]
                |> fun part -> part.Trim()  // Trim any leading/trailing whitespace

            // Check for username
            let userNameOption = 
                if String.IsNullOrEmpty(userName) then 
                    None 
                else 
                    Some userName

            // Borrow the book
            if Library.borrowBook bookTitle userNameOption then
                MessageBox.Show($"Book '{bookTitle}' borrowed successfully.") |> ignore
                booksListBox.Items.Remove(selectedBook)
            else
                MessageBox.Show("Borrowing failed.") |> ignore
        else
            MessageBox.Show("Please select a book to borrow.") |> ignore
    )


    // Add controls to the tab
    availableBooksTab.Controls.AddRange([| availableBooksButton; booksListBox; borrowButton |])



    let returnBooksTab = new TabPage("Return Books")
    returnBooksTab.BackColor <- Color.Honeydew

    let borrowedBooksButton = new Button(Text = "Show Borrowed Books", Top = 20, Left = 20, Width = 200, Height = 40)
    borrowedBooksButton.BackColor <- Color.LightCoral  // Changed to LightCoral
    let borrowedBooksListBox = new ListBox(Top = 70, Left = 20, Width = 700, Height = 250)

    let returnSelectedBookButton = new Button(Text = "Return Selected Book", Top = 330, Left = 20, Width = 200, Height = 40)
    returnSelectedBookButton.BackColor <- Color.LightCoral  // Changed to LightCoral

    borrowedBooksButton.Click.Add(fun _ -> 
        let borrowedBooks = 
            Library.getBorrowedBooksByUser userName
            |> Seq.map (fun book -> $"{book.Title} by {book.Author}")
            |> String.concat "\n"
        borrowedBooksListBox.Items.Clear()
        borrowedBooksListBox.Items.AddRange(borrowedBooks.Split('\n') |> Array.map (fun book -> box book))
    )

    returnSelectedBookButton.Click.Add(fun _ ->
        let selectedBook = borrowedBooksListBox.SelectedItem
        if selectedBook <> null then
            let bookTitle = selectedBook.ToString().Split(" by ").[0]
            if Library.returnBook bookTitle then
                MessageBox.Show($"Book '{bookTitle}' returned successfully.") |> ignore
                borrowedBooksListBox.Items.Remove(selectedBook)
            else
                MessageBox.Show("Returning failed.") |> ignore
        else
            MessageBox.Show("Please select a book to return.") |> ignore
    )

    returnBooksTab.Controls.AddRange([| borrowedBooksButton; borrowedBooksListBox; returnSelectedBookButton |])
    
    // Add Tabs to TabControl
    tabControl.TabPages.Add(borrowReturnTab)
    tabControl.TabPages.Add(availableBooksTab)
    tabControl.TabPages.Add(returnBooksTab)

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

   // Signup Button with hover effect and icon (side by side with login button)
    let signupButton = new Button(Text = "Signup", Top = 200, Left = (form.Width / 2) - 210, Width = 200, Height = 50)
    signupButton.Font <- new Font("Arial", 14.0f, FontStyle.Bold)
    signupButton.BackColor <- Color.CadetBlue
    signupButton.ForeColor <- Color.White
    signupButton.FlatStyle <- FlatStyle.Flat
    signupButton.FlatAppearance.BorderSize <- 0
    signupButton.FlatAppearance.MouseOverBackColor <- Color.FromArgb(100, 193, 150)
    signupButton.Click.Add(fun _ -> showSignupForm() |> ignore)
    form.Controls.Add(signupButton)
    
    // Login Button with hover effect and icon (side by side with signup button)
    let loginButton = new Button(Text = "Login", Top = 200, Left = (form.Width / 2) + 10, Width = 200, Height = 50)
    loginButton.Font <- new Font("Arial", 14.0f, FontStyle.Bold)
    loginButton.BackColor <- Color.CadetBlue
    loginButton.ForeColor <- Color.White
    loginButton.FlatStyle <- FlatStyle.Flat
    loginButton.FlatAppearance.BorderSize <- 0
    loginButton.FlatAppearance.MouseOverBackColor <- Color.FromArgb(100, 193, 150)
    loginButton.Click.Add(fun _ -> 
        let (userType, userName) = showLoginForm()  // Display login form and get user type
        match userType with
        | Some Admin -> showAdminPage() |> ignore  // Admin page with full functionality
        | Some User -> showUserPage(userName) |> ignore // User page with limited functionality
        | None -> ()  // Do nothing if the form is closed without login
    )
    form.Controls.Add(loginButton)
    // Add buttons and labels to the form
    form.Controls.Add(loginButton)
    form.ShowDialog()

// Main Application Flow
[<STAThread>]
do
    showHomePage()  // Display the home page


