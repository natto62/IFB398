<?php
	include 'DbDetails.php';
	//retrive post values from Settings.cs
	$email = $_POST["Email"];
	$opwd = $_POST["OPassword"];
	$npwd = $_POST["NPassword"];
	$npwdc = $_POST["NPasswordC"];
	
	$sqlQuery = "SELECT * FROM micaredb.users WHERE Email='" . $email . "';";
	//used to be able to send special characters to the database
	$options = array(PDO::MYSQL_ATTR_INIT_COMMAND => 'SET NAMES utf8'); 
	$hash="";

	try { 
		$db = new PDO("mysql:host={$host};dbname={$dbname};charset=utf8", $username, $password, $options); 
		$statement = $db->prepare($sqlQuery);
		$statement->execute();
		$results = $statement->fetchAll(PDO::FETCH_ASSOC);
		$hash = htmlentities($results[0]['Password']);
		//then check passwords
		if (password_verify($opwd,$hash)){
			if ($npwd == $npwdc) {
				$pwd = password_hash($npwd, PASSWORD_BCRYPT);//hash password
				$passsqlQuery = "UPDATE micaredb.users SET Password = '" . $pwd . "' WHERE Email='" . $email . "';";
				$newstatement = $db->prepare($passsqlQuery);
				$newstatement->execute();
				echo "Password has been changed.";
			} else {
				echo "Sorry, the given confirm password does not match up.";
			}
		} else {
			echo "Sorry, the given original password is incorrect.";
		}
	} catch(PDOException $ex) { 
		echo "Sorry, failed to connect to the database.";
	} 
?>
