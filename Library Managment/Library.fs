namespace LibraryManagement
open System
open System.IO
open Newtonsoft.Json

module Library =


    type Book = 
        { 
            Title: string
            Author: string
            Genre: string
            Price: string
            IsBorrowed: bool
            BorrowedDate: Option<DateTime> 
        }

    let libraryFilePath = "library.json"
    let revenueFilePath = "revenue.json"

    // Helper to save library data to a JSON file
    let private saveLibraryToFile (library: Map<string, Book>) =
        let json = JsonConvert.SerializeObject(library)
        File.WriteAllText(libraryFilePath, json)

    // Helper to load library data from a JSON file
    let private loadLibraryFromFile () =
        if File.Exists(libraryFilePath) then
            let json = File.ReadAllText(libraryFilePath)
            JsonConvert.DeserializeObject<Map<string, Book>>(json)
        else
            Map.empty

    // Helper to load total revenue from a JSON file
    let private loadRevenueFromFile () =
        if File.Exists(revenueFilePath) then
            let json = File.ReadAllText(revenueFilePath)
            match Decimal.TryParse(json) with
            | (true, revenue) -> revenue
            | _ -> 0M
        else
            0M

    // Helper to save total revenue to a JSON file
    let private saveRevenueToFile (revenue: decimal) =
        let json = revenue.ToString()
        File.WriteAllText(revenueFilePath, json)

    let addBook title author genre price =
        let library = loadLibraryFromFile()
        let book = { Title = title; Author = author; Genre = genre; Price = price; IsBorrowed = false; BorrowedDate = None }
        let updatedLibrary = library.Add(title, book)
        saveLibraryToFile updatedLibrary

    let searchBook title =
        let library = loadLibraryFromFile()
        library.TryFind(title)

    let borrowBook title =
        let library = loadLibraryFromFile()
        match library.TryFind(title) with
        | Some book when not book.IsBorrowed ->
            let updatedBook = { book with IsBorrowed = true; BorrowedDate = Some(DateTime.Now) }
            let updatedLibrary = library.Add(title, updatedBook)
            saveLibraryToFile updatedLibrary
            true
        | _ -> false

    let returnBook title =
        let library = loadLibraryFromFile()
        match library.TryFind(title) with
        | Some book when book.IsBorrowed ->
            match book.BorrowedDate with
            | Some borrowedDate ->
                let daysBorrowed = (DateTime.Now - borrowedDate).Days
                let dailyRate = 
                    match Decimal.TryParse(book.Price) with
                    | (true, price) -> price * 0.01M // Assuming 1% of the book's price per day
                    | _ -> 0M
                let cost = dailyRate * decimal daysBorrowed
                printfn "The cost of borrowing '%s' for %d day(s) is: %M" title daysBorrowed cost

                // Update total revenue
                let currentRevenue = loadRevenueFromFile()
                let updatedRevenue = currentRevenue + cost
                saveRevenueToFile updatedRevenue
                printfn "Total revenue updated: %M" updatedRevenue

                // Update the book status
                let updatedBook = { book with IsBorrowed = false; BorrowedDate = None }
                let updatedLibrary = library.Add(title, updatedBook)
                saveLibraryToFile updatedLibrary
                true
            | None -> 
                printfn "Error: BorrowedDate is missing."
                false
        | _ -> 
            printfn "Error: Book not found or not currently borrowed."
            false

    let getBooks () =
        let library = loadLibraryFromFile()
        library |> Map.toSeq |> Seq.map snd
