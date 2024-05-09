# FireChat
 
This is a chat app for Windows made as a for practice and example using WPF and Firebase Realtime Database with FireSharp.  
The whole techstack used and images of the app as well as the database structure are listed below.   

## Frontend
Windows Presentation Foundation (.Net Framework 4.8) themed with [MahApps.Metro](https://github.com/MahApps/MahApps.Metro).  

I chose WPF for this demo as it doesn't need to be crossplatform and it is better than alternatives (winforms) for rapid development, though I would use [Avalonia](https://avaloniaui.net/) if it needed to be crossplatform.  
The reason I chose Net Framework 4.8 for WPF is it is bundled with all modern Windoww machines so I don't need to package the runtime with the app to make it self-contained. Also [build tools/extensions of my choice](https://github.com/Fody/Costura) help me make the build super compact.

## Backend
I used [Firebase Realtime Database](https://firebase.google.com/docs/database) with the [FireSharp](https://github.com/bugthesystem/FireSharp) as the API.  

Most of the backend is implemented in a single class called [ChatDatabase](https://github.com/SametHope/FireChat/blob/main/Database/ChatDatabase.cs) which uses FireSharp features such as `FirebaseConfig`, `FirebaseClient`, `Client.GetAsync`, `Client.SetAsync`, `Client.DeleteAsync`, `Client.PushAsync` and `Client.OnAsync` which is most of there is on the API.  

I hash and salt the password using [Bcrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net) and don't store them as raw text on the database because I find it important even for a demo.

## Images

### Database Structure
![Ekran görüntüsü 2024-05-09 112619](https://github.com/SametHope/FireChat/assets/85421686/a9696d7b-3033-4721-a186-88aecd0382d1)  
### Login Window
![Ekran görüntüsü 2024-05-09 105343](https://github.com/SametHope/FireChat/assets/85421686/c8439d45-578f-422b-8aec-6c2941eb7ed0)  
### Register Window
![Ekran görüntüsü 2024-05-09 105501](https://github.com/SametHope/FireChat/assets/85421686/0b288945-db68-408e-9ad9-66d797ad4213)  
### Chat Window
You can see 2 messages sent by the currently logged in client. The username and the nickname is visible on the title.  
You can also choose to set your nickname as "anonymous" on your messages if you want to. The actual senders username is still sent to the database.  
  
![Ekran görüntüsü 2024-05-09 105726](https://github.com/SametHope/FireChat/assets/85421686/e30cc549-be2f-4c3a-a28d-81808be7d56f)  
### Chat Window Fullscreen
I used the Grid control to scale up the height of the messages ListView when relevant so more messages can be viewed on fullscreen and made the app more responsive.  
Also noticee this image is from another client with a different logged in account.  
  
![Ekran görüntüsü 2024-05-09 110106](https://github.com/SametHope/FireChat/assets/85421686/2decfd35-4573-4111-b790-d5ceb297a06a)  
### No Connection Screen
This screen is displayed if the connection with the database is not established.  
  
![Ekran görüntüsü 2024-05-09 110222](https://github.com/SametHope/FireChat/assets/85421686/8a41ffb8-df6a-41af-b171-ba00cac92183)  

# Notes
The secret key and database path is hardcoded. This is fine for me as my account is a spark aka free account so the server will simply deny requests if limits are reached.  
I did not configure any permissions so anyone can do anything with this knowledge, keep this in mind when submitting any information to the database and **do not use your real passwords**.
