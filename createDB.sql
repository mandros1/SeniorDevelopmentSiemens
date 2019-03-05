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
-- Table structure for table `level`
--

DROP TABLE IF EXISTS `level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `level` (
  `level_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `level` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`level_id`),
  UNIQUE KEY `pid_pk_UNIQUE` (`level_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Temporary view structure for view `log_viewer`
--

DROP TABLE IF EXISTS `log_viewer`;
/*!50001 DROP VIEW IF EXISTS `log_viewer`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE VIEW `log_viewer` AS SELECT 
 1 AS `timestamp`,
 1 AS `pid`,
 1 AS `tid`,
 1 AS `level`,
 1 AS `process_name`,
 1 AS `process_num`,
 1 AS `process_short`,
 1 AS `WSP`,
 1 AS `WSPPeak`,
 1 AS `HC`,
 1 AS `HCPeak`,
 1 AS `TC`,
 1 AS `TCPeak`,
 1 AS `CPU`,
 1 AS `CPUPeak`,
 1 AS `GDIC`,
 1 AS `GDICPeak`,
 1 AS `USRC`,
 1 AS `USRCPeak`,
 1 AS `PRIV`,
 1 AS `PRIVPeak`,
 1 AS `VIRT`,
 1 AS `VIRTPeak`,
 1 AS `PFS`,
 1 AS `PFSPeak`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `mri_data`
--

DROP TABLE IF EXISTS `mri_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mri_data` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `time_fk` int(10) unsigned DEFAULT NULL,
  `pid_fk` int(10) unsigned DEFAULT NULL,
  `tid_fk` int(10) unsigned DEFAULT NULL,
  `level_fk` int(10) unsigned DEFAULT NULL,
  `userText_fk` int(10) unsigned DEFAULT NULL,
  `process_fk` int(10) unsigned DEFAULT NULL,
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
  KEY `pid_fk_idx` (`pid_fk`),
  KEY `tid_fk_idx` (`tid_fk`),
  KEY `level_fk_idx` (`level_fk`),
  KEY `userText_fk_idx` (`userText_fk`),
  KEY `process_fk_idx` (`process_fk`),
  CONSTRAINT `level_fk` FOREIGN KEY (`level_fk`) REFERENCES `level` (`level_id`) ON DELETE NO ACTION ON UPDATE CASCADE,
  CONSTRAINT `pid_fk` FOREIGN KEY (`pid_fk`) REFERENCES `pid` (`pid_id`) ON DELETE NO ACTION ON UPDATE CASCADE,
  CONSTRAINT `process_fk` FOREIGN KEY (`process_fk`) REFERENCES `process` (`process_id`) ON DELETE NO ACTION ON UPDATE CASCADE,
  CONSTRAINT `tid_fk` FOREIGN KEY (`tid_fk`) REFERENCES `tid` (`tid_id`) ON DELETE NO ACTION ON UPDATE CASCADE,
  CONSTRAINT `time_fk` FOREIGN KEY (`time_fk`) REFERENCES `time` (`time_id`) ON DELETE NO ACTION ON UPDATE CASCADE,
  CONSTRAINT `userText_fk` FOREIGN KEY (`userText_fk`) REFERENCES `usertext` (`userText_id`) ON DELETE NO ACTION ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pid`
--

DROP TABLE IF EXISTS `pid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pid` (
  `pid_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `pid` mediumint(8) unsigned DEFAULT NULL,
  PRIMARY KEY (`pid_id`),
  UNIQUE KEY `pid_pk_UNIQUE` (`pid_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `process`
--

DROP TABLE IF EXISTS `process`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `process` (
  `process_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `process_name` varchar(255) DEFAULT NULL,
  `process_num` mediumint(8) unsigned DEFAULT NULL,
  `process_short` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`process_id`),
  UNIQUE KEY `process_name_UNIQUE` (`process_name`),
  UNIQUE KEY `process_num_UNIQUE` (`process_num`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tid`
--

DROP TABLE IF EXISTS `tid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tid` (
  `tid_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `tid` mediumint(8) unsigned DEFAULT NULL,
  PRIMARY KEY (`tid_id`),
  UNIQUE KEY `pid_pk_UNIQUE` (`tid_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;
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
) ENGINE=InnoDB AUTO_INCREMENT=138 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `usertext`
--

DROP TABLE IF EXISTS `usertext`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `usertext` (
  `userText_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `userText` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`userText_id`),
  UNIQUE KEY `pid_pk_UNIQUE` (`userText_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Final view structure for view `log_viewer`
--

/*!50001 DROP VIEW IF EXISTS `log_viewer`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `log_viewer` AS select `t`.`timeStamp` AS `timestamp`,`p`.`pid` AS `pid`,`tid`.`tid` AS `tid`,`l`.`level` AS `level`,`pro`.`process_name` AS `process_name`,`pro`.`process_num` AS `process_num`,`pro`.`process_short` AS `process_short`,`mri`.`WSP` AS `WSP`,`mri`.`WSPPeak` AS `WSPPeak`,`mri`.`HC` AS `HC`,`mri`.`HCPeak` AS `HCPeak`,`mri`.`TC` AS `TC`,`mri`.`TCPeak` AS `TCPeak`,`mri`.`CPU` AS `CPU`,`mri`.`CPUPeak` AS `CPUPeak`,`mri`.`GDIC` AS `GDIC`,`mri`.`GDICPeak` AS `GDICPeak`,`mri`.`USRC` AS `USRC`,`mri`.`USRCPeak` AS `USRCPeak`,`mri`.`PRIV` AS `PRIV`,`mri`.`PRIVPEAK` AS `PRIVPeak`,`mri`.`VIRT` AS `VIRT`,`mri`.`VIRTPeak` AS `VIRTPeak`,`mri`.`PFS` AS `PFS`,`mri`.`PFSPeak` AS `PFSPeak` from (((((`mri_data` `mri` join `time` `t` on((`mri`.`time_fk` = `t`.`time_id`))) join `pid` `p` on((`mri`.`pid_fk` = `p`.`pid_id`))) join `tid` on((`mri`.`tid_fk` = `tid`.`tid_id`))) join `level` `l` on((`mri`.`level_fk` = `l`.`level_id`))) join `process` `pro` on((`mri`.`process_fk` = `pro`.`process_id`))) */;
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

-- Dump completed on 2019-03-03  2:00:55
