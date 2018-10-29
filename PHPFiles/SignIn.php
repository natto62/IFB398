<?php
	include 'DbDetails.php';
	//retrive post values from SignIn.cs
	$email = $_POST["Email"];
	$pwd = $_POST["Password"];
	
	$sqlQuery = "SELECT * FROM micaredb.users WHERE Email='" . $email . "';";
 
	$hash="";

	try { 
		$db = new PDO("mysql:host={$host};dbname={$dbname};charset=utf8", $username, $password); 
		$statement = $db->prepare($sqlQuery);
		$statement->execute();
		$results = $statement->fetchAll(PDO::FETCH_ASSOC);
		//if no email
		if ($results==null){
			echo "Sorry, that email does not exist in our systems.";
		//if email exists
		} else {
			$hash = htmlentities($results[0]['Password']);
			//then check passwords
			if (password_verify($pwd,$hash)){
				echo json_encode($results);
			} else {
				echo "Sorry, the given password is incorrect.";
			}
		}
	} catch(PDOException $ex) { 
		echo "Sorry, failed to connect to the database.";
	} 
?>
