#if !UNITY_WEBGL
using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
#endif
using UnityEngine;

public class FirebaseConnector : MonoBehaviour
{
#if !UNITY_WEBGL
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
#endif
}
