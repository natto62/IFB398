<?php
	include 'DbDetails.php';
	//retrive post values from SignUp.cs
	$fname = $_POST["FName"];
	$lname = $_POST["LName"];
	$email = $_POST["Email"];
	$passWd = $_POST["Password"];
	
	$pwd = password_hash($passWd, PASSWORD_BCRYPT);//hash password
	
	$sqlQuery = "SELECT * FROM micaredb.users WHERE Email='" . $email . "';";
	
	$sqlQuerySecond = "INSERT INTO `micaredb`.`users` (`FName`, `LName`, `Email`, `Password`) VALUES ('" . $fname . "', '" . $lname . "', '" . $email . "', '" . $pwd . "');";
	//used to be able to send special characters to the database
	$options = array(PDO::MYSQL_ATTR_INIT_COMMAND => 'SET NAMES utf8'); 

	try { 
		$db = new PDO("mysql:host={$host};dbname={$dbname};charset=utf8", $username, $password, $options); 
		$statement = $db->prepare($sqlQuery);
		$statement->execute();
		$results = $statement->fetchAll(PDO::FETCH_ASSOC);
		//if email doesn't exist create user
		if ($results==null){
			$statement = $db->prepare($sqlQuerySecond);
			$statement->execute();
			echo "Sign Up Complete.";
		//if it already exists, tell the user
		} else {
			echo "Sorry, that email already exists in our systems.";
		}		
	} catch(PDOException $ex) { 
		echo "Sorry, failed to connect to the database."; 
	} 
?>
