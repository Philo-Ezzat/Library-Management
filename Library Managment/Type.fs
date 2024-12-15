namespace LibraryManagement

open System

type Book = { Title: string; Author: string; Genre: string;Price: string; IsBorrowed: bool; BorrowedDate: Option<DateTime> }