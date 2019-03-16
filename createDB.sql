CREATE DATABASE  IF NOT EXISTS `mri` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `mri`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: mri
-- ------------------------------------------------------
-- Server version	5.7.21-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `mri_data`
--

DROP TABLE IF EXISTS `mri_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mri_data` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `time_fk` varchar(255) DEFAULT NULL,
  `process_fk` varchar(255) DEFAULT NULL,
  `WSP` double unsigned DEFAULT NULL,
  `WSPPeak` double unsigned DEFAULT NULL,
  `HC` int(11) unsigned DEFAULT NULL,
  `HCPeak` int(11) unsigned DEFAULT NULL,
  `TC` int(11) unsigned DEFAULT NULL,
  `TCPeak` int(11) unsigned DEFAULT NULL,
  `CPU` int(11) unsigned DEFAULT NULL,
  `CPUPeak` int(11) unsigned DEFAULT NULL,
  `GDIC` int(11) unsigned DEFAULT NULL,
  `GDICPeak` int(11) unsigned DEFAULT NULL,
  `USRC` int(11) unsigned DEFAULT NULL,
  `USRCPeak` int(11) unsigned DEFAULT NULL,
  `PRIV` double unsigned DEFAULT NULL,
  `PRIVPEAK` double unsigned DEFAULT NULL,
  `VIRT` double unsigned DEFAULT NULL,
  `VIRTPeak` double unsigned DEFAULT NULL,
  `PFS` double unsigned DEFAULT NULL,
  `PFSPeak` double unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `time_fk_idx` (`time_fk`),
  KEY `process_fk_idx` (`process_fk`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Temporary view structure for view `mri_view`
--

DROP TABLE IF EXISTS `mri_view`;
/*!50001 DROP VIEW IF EXISTS `mri_view`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE VIEW `mri_view` AS SELECT 
 1 AS `timestamp`,
 1 AS `process_name`,
 1 AS `WSPPEAK`,
 1 AS `HC`,
 1 AS `HCPEAK`,
 1 AS `TC`,
 1 AS `TCPEAK`,
 1 AS `CPU`,
 1 AS `CPUPEAK`,
 1 AS `GDIC`,
 1 AS `GDICPEAK`,
 1 AS `USRC`,
 1 AS `USRCPEAK`,
 1 AS `PRIV`,
 1 AS `PRIVPEAK`,
 1 AS `VIRT`,
 1 AS `VIRTPEAK`,
 1 AS `PFS`,
 1 AS `PFSPEAK`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `process`
--

DROP TABLE IF EXISTS `process`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `process` (
  `process_id` int(100) UNSIGNED NOT NULL AUTO_INCREMENT,
  `process_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`process_id`),
  UNIQUE KEY `process_name_UNIQUE` (`process_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `time`
--

DROP TABLE IF EXISTS `time`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `time` (
  `time_id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `timeStamp` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`time_id`),
  UNIQUE KEY `timeStamp_UNIQUE` (`timeStamp`)
) ENGINE=InnoDB AUTO_INCREMENT=3941 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Final view structure for view `mri_view`
--

/*!50001 DROP VIEW IF EXISTS `mri_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `mri_view` AS select `t`.`timeStamp` AS `timestamp`,`p`.`process_name` AS `process_name`,`m`.`WSPPeak` AS `WSPPEAK`,`m`.`HC` AS `HC`,`m`.`HCPeak` AS `HCPEAK`,`m`.`TC` AS `TC`,`m`.`TCPeak` AS `TCPEAK`,`m`.`CPU` AS `CPU`,`m`.`CPUPeak` AS `CPUPEAK`,`m`.`GDIC` AS `GDIC`,`m`.`GDICPeak` AS `GDICPEAK`,`m`.`USRC` AS `USRC`,`m`.`USRCPeak` AS `USRCPEAK`,`m`.`PRIV` AS `PRIV`,`m`.`PRIVPEAK` AS `PRIVPEAK`,`m`.`VIRT` AS `VIRT`,`m`.`VIRTPeak` AS `VIRTPEAK`,`m`.`PFS` AS `PFS`,`m`.`PFSPeak` AS `PFSPEAK` from ((`mri_data` `m` join `process` `p` on((`m`.`process_fk` = `p`.`process_id`))) join `time` `t` on((`m`.`time_fk` = `t`.`time_id`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-03-07  2:46:35
