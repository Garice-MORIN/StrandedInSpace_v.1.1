<html lang="fr">
<head>
    <link href="..\pages_css\accueil.css" rel="stylesheet" type="text/css" />
    <meta charset="utf-8" />
    <title>Stranded In Space</title>
</head>
<body>
    <nav class="menu" style="height: 90px;">
        <img src="..\images\popcorn1.png" class="pop">
        <div class="logo">
            <a href="../index.html">Stranded In Space</a>
        </div>
        <ul class="links">
            <li>
                <a href="lejeu.html">Le jeu</a>
                <ul class="submenu">
                    <li> <a href="#histoire">- histoire</a></li>
                    <li> <a href="#">- principe</a></li>
                    <li> <a href="#">- avancement</a></li>
                    <li> <a href="#">- jeu</a></li>
                </ul>
            </li>
            <li>
                <a href="histoire.html">L'histoire du groupe</a>
                <ul class="submenu">
                    <li> <a href="#">- Histoire</a></li>
                    <li> <a href="#">- les membres</a></li>
                    <li> <a href="#"></a></li>
                </ul>
            </li>
            <li>
                <a href="#">Highscores</a>
            </li>
        </ul>
        <img src="..\images\popcorn2.png" class="pop">
    </nav>
    </br></br>
    <p class="text">
        Connexion :
    </p>
    </br></br>
    <p class="text">
        <form action="connexion.php" method="post">
            Pseudo: <input type="text" name="pseudo" value="" required/>
            <br />
            Mot de passe: <input type="password" name="mdp" value="" required/>
            <br />
            <br />
            <input type="submit" name="connexion" value="Connexion" />
        </form>
    </p>
    <p class="cobutton">
        <a href="inscription.php">Pas encore inscrit ?</a>
    </p>
</body>
</html>