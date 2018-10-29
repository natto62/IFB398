<?php
	include 'DbDetails.php';
	//retrive post value from a kpi .cs file
	$type = $_POST["Type"];
	
	$sqlQuery = "";
	//this switch case will dtermine which table to pull data from
	switch ($type) {
		case "Agency":
			$sqlQuery = "SELECT * FROM micaredb.agency_usage;";
			break;
		case "Brokerage":
			$sqlQuery = "SELECT * FROM micaredb.brokerage_hours;";
			break;
		case "Bank":
			$sqlQuery = "SELECT * FROM micaredb.bank_balance;";
			break;
		case "ACFI":
			$sqlQuery = "SELECT * FROM micaredb.acfi_funding;";
			break;
		case "Home":
			$sqlQuery = "SELECT * FROM micaredb.home_care_packages;";
			break;
		case "Income":
			$sqlQuery = "SELECT * FROM micaredb.income;";
			break;
		case "Occupancy":
			$sqlQuery = "SELECT * FROM micaredb.occupancy;";
			break;
		case "Salaries":
			$sqlQuery = "SELECT * FROM micaredb.salaries_wages;";
			break;
		case "Staff":
			$sqlQuery = "SELECT * FROM micaredb.staff;";
			break;
	}

	try { 
		$db = new PDO("mysql:host={$host};dbname={$dbname};charset=utf8", $username, $password); 
		$statement = $db->prepare($sqlQuery);
		$statement->execute();
		$results = $statement->fetchAll(PDO::FETCH_ASSOC);
		echo json_encode($results);
	} catch(PDOException $ex) { 
		echo "Sorry, failed to connect to the database.";
	} 
?>