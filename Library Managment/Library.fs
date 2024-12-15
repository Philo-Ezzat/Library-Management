namespace LibraryManagement
open System
open System.IO

module Library =
    type Book = 
        { 
            Title: string
            Author: string
            Genre: string
            IsBorrowed: bool
            BorrowedDate: Option<DateTime> 
        }

    // File path to store library data
    let libraryFilePath = "library.txt"

    // Helper to save library to file
    let private saveLibraryToFile (library: Map<string, Book>) =
        let lines =
            library
            |> Map.toSeq
            |> Seq.map (fun (_, book) -> 
                $"""{book.Title}|{book.Author}|{book.Genre}|{book.IsBorrowed}|{match book.BorrowedDate with Some date -> date.ToString("o") | None -> ""}""")
        File.WriteAllLines(libraryFilePath, lines)

    // Helper to load library from file
    let private loadLibraryFromFile () =
        if File.Exists(libraryFilePath) then
            let lines = File.ReadAllLines(libraryFilePath)
            lines
            |> Seq.map (fun line ->
                let parts = line.Split('|')
                let borrowedDate = 
                    if parts.Length > 4 && not (String.IsNullOrWhiteSpace(parts.[4])) then 
                        Some(DateTime.Parse(parts.[4])) 
                    else 
                        None
                { Title = parts.[0]; Author = parts.[1]; Genre = parts.[2]; IsBorrowed = Boolean.Parse(parts.[3]); BorrowedDate = borrowedDate })
            |> Seq.map (fun book -> book.Title, book)
            |> Map.ofSeq
        else
            Map.empty // Return an empty map if the file does not exist

    // Add a new book
    let addBook title author genre =
        let library = loadLibraryFromFile() // Load the latest state
        let book = { Title = title; Author = author; Genre = genre; IsBorrowed = false; BorrowedDate = None }
        let updatedLibrary = library.Add(title, book)
        saveLibraryToFile updatedLibrary

    // Search for a book by title
    let searchBook title =
        let library = loadLibraryFromFile() // Load the latest state
        library.TryFind(title)

    // Borrow a book
    let borrowBook title =
        let library = loadLibraryFromFile() // Load the latest state
        match library.TryFind(title) with
        | Some book when not book.IsBorrowed ->
            let updatedBook = { book with IsBorrowed = true; BorrowedDate = Some(DateTime.Now) }
            let updatedLibrary = library.Add(title, updatedBook)
            saveLibraryToFile updatedLibrary
            true
        | _ -> false // Either book not found or already borrowed

    // Return a book
    let returnBook title =
        let library = loadLibraryFromFile() // Load the latest state
        match library.TryFind(title) with
        | Some book when book.IsBorrowed ->
            let updatedBook = { book with IsBorrowed = false; BorrowedDate = None }
            let updatedLibrary = library.Add(title, updatedBook)
            saveLibraryToFile updatedLibrary
            true
        | _ -> false // Either book not found or not borrowed

    // Get all books
    let getBooks () =
        let library = loadLibraryFromFile() // Load the latest state
        library |> Map.toSeq |> Seq.map snd
