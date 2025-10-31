Spotify -> Tidal playlist transfer fullstack application (still in development) 

Full‑stack example app (.NET backend + Vite + React + TypeScript frontend) demonstrating OAuth 2.0 (PKCE) flows. The main purpose of this application is for learning.
As such from this project I hope to better understand:

- Exception handling
- Authenthication flows
- Work with APIs
- Fullstack application development
- Tanstack router

The app itself lets the user to login to their spotify and tidal accounts, they then are able to pick and choose their playlists for transfering. The track matching works by comparing ISRC codes.

The app is controlled trough four main screens given to the user:

Main Screen:
<img width="956" height="449" alt="spotify1" src="https://github.com/user-attachments/assets/66020dd3-b1da-4421-b18f-a3ba7e8ff6eb" />

Source pick screen:
<img width="959" height="449" alt="spotify2" src="https://github.com/user-attachments/assets/c34adec5-7e1e-47b6-9287-1fab3488d9e8" />

Destination pick screen:
<img width="959" height="448" alt="spotify3" src="https://github.com/user-attachments/assets/b85e81f5-8810-4d9f-b52b-6eeabf7a5ba8" />

Playlist managment screen
<img width="947" height="449" alt="spotify4" src="https://github.com/user-attachments/assets/4a9a65e1-681e-418b-953c-aa927870bea3" />

TODO:
1. Better error and loading representation in transfer screen.
2. Add track matching other than ISRC comparison
3. Add more services
