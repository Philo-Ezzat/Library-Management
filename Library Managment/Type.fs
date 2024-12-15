namespace LibraryManagement

open System

type Book = { Title: string; Author: string; Genre: string; IsBorrowed: bool; BorrowedDate: Option<DateTime> }