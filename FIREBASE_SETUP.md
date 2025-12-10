# Firebase Setup Guide for ErgoHealthCue Leaderboard

## ✅ Firebase Already Configured

**Good news!** The Firebase Realtime Database is already set up and configured for ErgoHealthCue. The leaderboard will work out of the box without any additional setup required.

- **Firebase URL**: `https://ergohealthcue-default-rtdb.europe-west1.firebasedatabase.app/`
- **Region**: europe-west1
- **Status**: Ready to use

## How to Use the Leaderboard

1. **First Launch**: When you start ErgoHealthCue for the first time, you'll be prompted to enter a username
2. **Optional**: You can leave the username empty to auto-generate one, or opt-out of the leaderboard entirely
3. **View Rankings**: Access the leaderboard from the system tray menu or Settings window
4. **Privacy**: You can disable the leaderboard at any time in Settings

---

## For Developers: Setting Up Your Own Firebase Instance

If you want to set up your own Firebase Realtime Database instance for development or forking purposes, follow these steps:

### Step 1: Create a Firebase Project

1. Go to the [Firebase Console](https://console.firebase.google.com/)
2. Click **"Add project"** or select an existing project
3. Enter a project name (e.g., "ErgoHealthCue")
4. Follow the setup wizard (you can disable Google Analytics if you don't need it)
5. Click **"Create project"**

## Step 2: Create a Realtime Database

1. In your Firebase project console, click on **"Realtime Database"** in the left sidebar (under "Build")
2. Click **"Create Database"**
3. Choose a database location (select the closest to your users)
4. Start in **"Test mode"** for now (we'll configure security rules next)
5. Click **"Enable"**

## Step 3: Configure Database Security Rules

1. In the Realtime Database page, click on the **"Rules"** tab
2. Replace the default rules with the following:

```json
{
  "rules": {
    "leaderboard": {
      ".read": true,
      ".indexOn": ["Level", "TotalXP"],
      "$userId": {
        ".write": true,
        ".validate": "newData.hasChildren(['UserId', 'Username', 'Level', 'TotalXP', 'CompletedCues', 'DismissedCues', 'LastUpdated'])"
      }
    }
  }
}
```

These rules:
- Allow anyone to **read** the leaderboard (so users can see rankings)
- Allow users to **write** their own data (identified by userId)
- Index by Level and TotalXP for efficient sorting
- Validate that all required fields are present

3. Click **"Publish"**

## Step 4: Get Your Database URL

1. In the Realtime Database page, look at the top of the page
2. You'll see your database URL. Depending on your region, it will look like one of these:
   - US/Global: `https://YOUR-PROJECT-ID-default-rtdb.firebaseio.com/`
   - Europe: `https://YOUR-PROJECT-ID-default-rtdb.europe-west1.firebasedatabase.app/`
   - Asia: `https://YOUR-PROJECT-ID-default-rtdb.asia-southeast1.firebasedatabase.app/`
3. Copy this URL

## Step 5: Update the Application Code

⚠️ **IMPORTANT**: You must update the Firebase URL in the code to use your own database.

1. Open the file: `ErgoHealthCue/Services/LeaderboardService.cs`
2. Find line 13 where the FIREBASE_URL constant is defined:
   ```csharp
   private const string FIREBASE_URL = "https://ergohealthcue-default-rtdb.europe-west1.firebasedatabase.app/";
   private const string FIREBASE_URL = "https://ergohealthcue-default-rtdb.europe-west1.firebasedatabase.app/";
   ```
3. Replace the entire URL with your own Firebase Realtime Database URL from Step 4
   - Example: If your URL is `https://my-ergo-app-default-rtdb.europe-west1.firebasedatabase.app/`, update to:
   ```csharp
   private const string FIREBASE_URL = "https://my-ergo-app-default-rtdb.europe-west1.firebasedatabase.app/";
   ```

## Step 6: Build and Test

1. Build the application:
   ```bash
   dotnet build ErgoHealthCue/ErgoHealthCue.csproj --configuration Release
   ```

2. Run the application and test the leaderboard:
   - Open Settings
   - Enable the Leaderboard checkbox
   - Enter a username
   - Save settings
   - Complete some cues
   - Open the Leaderboard from the system tray menu or Settings

## Optional: Improve Security (Recommended for Production)

For production use, consider implementing Firebase Authentication:

1. Enable Firebase Authentication in your Firebase Console
2. Update security rules to require authentication:
   ```json
   {
     "rules": {
       "leaderboard": {
         ".read": "auth != null",
         "$userId": {
           ".write": "auth != null && auth.uid == $userId"
         }
       }
     }
   }
   ```
3. Update the application code to authenticate users before accessing the database

## Firebase Free Tier Limits

The Firebase Realtime Database free tier (Spark Plan) includes:
- **1 GB stored data**
- **10 GB/month data transfer**
- **100 simultaneous connections**

This should be sufficient for thousands of users with the leaderboard feature.

## Troubleshooting

### "Permission denied" error
- Check that your database security rules allow read/write access
- Verify the rules are published

### "Network error" or "Unable to connect"
- Verify the FIREBASE_URL is correct in LeaderboardService.cs
- Check your internet connection
- Ensure the database is in the correct region

### Data not appearing in leaderboard
- Complete at least one cue to trigger a leaderboard update
- Check the Firebase Console to see if data is being written
- Verify "Enable Leaderboard" is checked in Settings

## Support

For more information about Firebase Realtime Database:
- [Firebase Realtime Database Documentation](https://firebase.google.com/docs/database)
- [Security Rules Guide](https://firebase.google.com/docs/database/security)

---

**Note**: The current implementation uses a placeholder Firebase URL. You MUST update it with your own Firebase project URL for the leaderboard to work.
