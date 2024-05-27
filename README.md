Cameron Liddell | S4103848 | CT6007
University of Gloucestershire

This is my individual research project practical. My IRP was about using Artificial intelligence (AI) in video games to improve player to NPC interactions and enhancing the experience for the player. This project demonstrates the uses of ChatGPT based NPCs for dialogue, interactions and quests and how they can change the way in which players navigate the game.

Known GitHub issue:
  - Upon pulling the project from GitHub, the project will flag that it cannot find a certain .tgz file. This is out of my control due to how InWorld is made.

Fix:
  - Find the project folder, and go to Packages -> Manifest.json
  - Find where it says "com.inworld.unity.core" and you'll need to change the package location.
  - Change the location for your PC to "file:{drive}:\\{download_folder}\\IRP\\Assets\\InWorld\\\\com.inworld.unity.core-3.3.2.tgz"
  - Launch Unity and ensure full package is downloaded and functioning


P.S this error is completely out of my control.
