<?php
	include 'DbDetails.php';
	//retrive post values from Settings.cs
	$email = $_POST["Email"];
	$fname = $_POST["FName"];
	$lname = $_POST["LName"];
	
	$sqlQuery = "UPDATE micaredb.users SET FName = '" . $fname . "', LName = '" . $lname . "' WHERE Email='" . $email . "';";
	//used to be able to send special characters to the database
	$options = array(PDO::MYSQL_ATTR_INIT_COMMAND => 'SET NAMES utf8'); 

	try { 
		$db = new PDO("mysql:host={$host};dbname={$dbname};charset=utf8", $username, $password, $options); 
		$statement = $db->prepare($sqlQuery);
		$statement->execute();
		echo "Name has been changed.";
	} catch(PDOException $ex) { 
		echo "Sorry, failed to connect to the database.";
	} 
?>
