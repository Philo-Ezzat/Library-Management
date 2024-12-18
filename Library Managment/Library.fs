﻿namespace LibraryManagement
open System
open System.IO
open Newtonsoft.Json
// library database
module Library =


    type Book = 
        { 
            Title: string
            Author: string
            Genre: string
            Price: string
            IsBorrowed: bool
            BorrowedDate: Option<DateTime> 
            Username: Option<string> 
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
    // add book
    let addBook title author genre price =
        let library = loadLibraryFromFile()
        let book = { Title = title; Author = author; Genre = genre; Price = price; IsBorrowed = false; BorrowedDate = None; Username = None }
        let updatedLibrary = library.Add(title, book)
        saveLibraryToFile updatedLibrary
    //search for a book
    let searchBook title =
        let library = loadLibraryFromFile()
        library.TryFind(title)
    //borrow a book
    let borrowBook title username =
        let library = loadLibraryFromFile()
        match library.TryFind(title) with
        | Some book when not book.IsBorrowed ->
            let updatedBook = { book with IsBorrowed = true; BorrowedDate = Some(DateTime.Now); Username = username  }
            let updatedLibrary = library.Add(title, updatedBook)
            saveLibraryToFile updatedLibrary
            true
        | _ -> false
    //get all the borrowed books
    let getBorrowedBooksByUser (username: string) =
        let library = loadLibraryFromFile()
        library |> Map.toSeq |> Seq.map snd |> Seq.filter (fun book -> book.Username = Some username)
    // return the book
    let returnBook title =
        let library = loadLibraryFromFile()
        match library.TryFind(title) with
        | Some book when book.IsBorrowed ->
            match book.BorrowedDate with
            | Some borrowedDate ->
                // Calculate the total time in days (fractional)
                let totalDaysBorrowed = (DateTime.Now - borrowedDate).TotalDays
                // Round up to the nearest whole day
                let daysBorrowed = int (Math.Ceiling(totalDaysBorrowed)) // 1 minute = 1 day, 1 day 1 minute = 2 days

                // Calculate the cost
                let dailyRate = 
                    match Decimal.TryParse(book.Price) with
                    | (true, price) -> price
                    | _ -> 0M
                let cost = dailyRate * decimal daysBorrowed
                printfn "The cost of borrowing '%s' for %d day(s) is: %M" title daysBorrowed cost

                // Update total revenue
                let currentRevenue = loadRevenueFromFile()
                let updatedRevenue = currentRevenue + cost
                saveRevenueToFile updatedRevenue
                printfn "Total revenue updated: %M" updatedRevenue

                // Update the book status
                let updatedBook = { book with IsBorrowed = false; BorrowedDate = None; Username = None }
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

    let searchBooksByName (name: string) : seq<Book> =
        let library = loadLibraryFromFile()
        library |> Map.toSeq |> Seq.map snd |> Seq.filter (fun book -> book.Title.Contains(name))