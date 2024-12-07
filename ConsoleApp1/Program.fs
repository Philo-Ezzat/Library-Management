open System

type Book = {
    Title: string
    Genre: string
    Price: string
    IsBorrowed: bool
    BorrowDate: Option<DateTime>
}

let mutable library = Map.empty<string, Book>

// Function Definitions
let addBook title price genre =
    let book = { Title = title; Price = price; Genre = genre; IsBorrowed = false; BorrowDate = None }
    library <- library.Add(title, book)

let searchBook title =
    match library.TryFind(title) with
    | Some book -> printfn "Found: %s (Genre: %s, Price: %s)" book.Title book.Genre book.Price; Some book
    | None -> printfn "Book not found"; None

let borrowBook title =
    match library.TryFind(title) with
    | Some book when not book.IsBorrowed ->
        let updatedBook = { book with IsBorrowed = true; BorrowDate = Some DateTime.Now }
        library <- library.Add(title, updatedBook)
        printfn "You borrowed: %s" title
    | Some _ -> printfn "The book is already borrowed."
    | None -> printfn "Book not found."

let returnBook title =
    match library.TryFind(title) with
    | Some book when book.IsBorrowed ->
        let updatedBook = { book with IsBorrowed = false; BorrowDate = None }
        library <- library.Add(title, updatedBook)
        printfn "You returned: %s" title
    | Some _ -> printfn "The book is not borrowed."
    | None -> printfn "Book not found."

let displayBooks () =
    printfn "Library Inventory:"
    library |> Map.iter (fun _ book ->
        printfn "- %s (Genre: %s, Price: %s, Status: %s)" 
            book.Title book.Genre book.Price (if book.IsBorrowed then "Borrowed" else "Available"))

// Test Cases with User Input
let rec mainMenu () =
    printfn "\n--- Library Management System ---"
    printfn "1. Add Book"
    printfn "2. Search Book"
    printfn "3. Borrow Book"
    printfn "4. Return Book"
    printfn "5. Display Books"
    printfn "6. Exit"
    printf "Choose an option: "
    let choice = Console.ReadLine()
    match choice with
    | "1" -> 
        printf "Enter book title: "
        let title = Console.ReadLine()
        printf "Enter book price: "
        let price = Console.ReadLine()
        printf "Enter book genre: "
        let genre = Console.ReadLine()
        addBook title price genre
        printfn "Book added successfully!"
        mainMenu()
    | "2" -> 
        printf "Enter book title to search: "
        let title = Console.ReadLine()
        searchBook title |> ignore
        mainMenu()
    | "3" -> 
        printf "Enter book title to borrow: "
        let title = Console.ReadLine()
        borrowBook title
        mainMenu()
    | "4" -> 
        printf "Enter book title to return: "
        let title = Console.ReadLine()
        returnBook title
        mainMenu()
    | "5" -> 
        displayBooks()
        mainMenu()
    | "6" -> printfn "Exiting program. Goodbye!"
    | _ -> 
        printfn "Invalid choice. Please try again."
        mainMenu()

// Start the Application
mainMenu()
