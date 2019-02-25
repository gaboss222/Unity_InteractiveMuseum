# Musée interactif
## Gabriel Griesser
### HE-ARC 2018

### Description

Ce projet Unity consiste à créer un musée 3D interactif dans lequel sera placé différentes salles.
Ces dernières correspondent chacunes à un auteur. Chaque salle contient les tableaux de l'auteur respectif.

Toute la problématique du projet repose sur
* La création dynamique des salles de tailles différentes (en fonction des tableaux)
* Le placement des tableaux dans ces salles
* Le placement des salles le long du corridor

### Utilisation

Il suffit d'importer un projet en Unity. A partie de là, tout se fera avec le mode "play" du logiciel.
L'objet RigidBodyFPSController a été directement importé des assets de Unity afin d'avoir une vue 1ère personne pour le projet.

* Les prefabs utilisés pour les GameObjets sont dans le dossier Prefabs
* Les matériels utilisés sont dans le dossier Materials
* Les textures pour les matériels sont dans le dossier Textures. A savoir que TOUTES les textures de l'ancien projet sont dedans.
* Les Scripts sont dans le dossier Scripts. (Le Script SimpleJSON est nécessaire à la bonne lecture du fichier dbv.json qui contient les données des tableaux)
* Standard Assets contient la vue 1ère personne de Unity.
* Les images des tableaux sont dans le dossier Art. Certaines salles reprennnent les images des précédents tableaux, cependant la taille est différente à chaque fois. Cela a été mis en place pour tester des salles avec beaucoup de tableaux sans forcément devoir télécharger l'image du tableau respectif sur le web.