using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseConnector : MonoBehaviour
{
    public static FirebaseApp app;

    void Start() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
            app = FirebaseApp.DefaultInstance;
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            Debug.Log("Firebase and Crashlytics initialized sucessfully");
        } else {
            Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        }
        });
    }
}
