# Mus�e interactif
## Gabriel Griesser
### HE-ARC 2018

### Description

Ce projet Unity consiste � cr�er un mus�e 3D interactif dans lequel sera plac� diff�rentes salles.
Ces derni�res correspondent chacunes � un auteur. Chaque salle contient les tableaux de l'auteur respectif.

Toute la probl�matique du projet repose sur
* La cr�ation dynamique des salles de tailles diff�rentes (en fonction des tableaux)
* Le placement des tableaux dans ces salles
* Le placement des salles le long du corridor

### Utilisation

Il suffit d'importer un projet en Unity. A partie de l�, tout se fera avec le mode "play" du logiciel.
L'objet RigidBodyFPSController a �t� directement import� des assets de Unity afin d'avoir une vue 1�re personne pour le projet.

* Les prefabs utilis�s pour les GameObjets sont dans le dossier Prefabs
* Les mat�riels utilis�s sont dans le dossier Materials
* Les textures pour les mat�riels sont dans le dossier Textures. A savoir que TOUTES les textures de l'ancien projet sont dedans.
* Les Scripts sont dans le dossier Scripts. (Le Script SimpleJSON est n�cessaire � la bonne lecture du fichier dbv.json qui contient les donn�es des tableaux)
* Standard Assets contient la vue 1�re personne de Unity.
* Les images des tableaux sont dans le dossier Art. Certaines salles reprennnent les images des pr�c�dents tableaux, cependant la taille est diff�rente � chaque fois. Cela a �t� mis en place pour tester des salles avec beaucoup de tableaux sans forc�ment devoir t�l�charger l'image du tableau respectif sur le web.