using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class UserModel
{
    [FirestoreProperty]
    public string email { get; set; }

    [FirestoreProperty]
    public string username { get; set; }

    [FirestoreProperty]
    public string role { get; set; }

    [FirestoreProperty]
    public string empID { get; set; }

    [FirestoreProperty]
    public Dictionary<string, double> bestTimes { get; set; }
}
