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
SET GLOBAL max_allowed_packet=1024*1024*1024;
--
-- Table structure for table `global0`
--

DROP TABLE IF EXISTS `global0`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `global0` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `TimeStamp` timestamp(6) NULL DEFAULT NULL,
  `FileName` varchar(45) NOT NULL,
  `GCPU0` double unsigned DEFAULT NULL,
  `GCPU0Peak` double unsigned DEFAULT NULL,
  `GCPU1` double unsigned DEFAULT NULL,
  `GCPU1Peak` double unsigned DEFAULT NULL,
  `GCPU2` double unsigned DEFAULT NULL,
  `GCPU2Peak` double unsigned DEFAULT NULL,
  `GCPU3` double unsigned DEFAULT NULL,
  `GCPU3Peak` double unsigned DEFAULT NULL,
  `GCPU4` double unsigned DEFAULT NULL,
  `GCPU4Peak` double unsigned DEFAULT NULL,
  `GCPU5` double unsigned DEFAULT NULL,
  `GCPU5Peak` double unsigned DEFAULT NULL,
  `GCPU6` double unsigned DEFAULT NULL,
  `GCPU6Peak` double unsigned DEFAULT NULL,
  `GCPU7` double unsigned DEFAULT NULL,
  `GCPU7Peak` double unsigned DEFAULT NULL,
  `GCPU8` double unsigned DEFAULT NULL,
  `GCPU8Peak` double unsigned DEFAULT NULL,
  `GCPU9` double unsigned DEFAULT NULL,
  `GCPU9Peak` double unsigned DEFAULT NULL,
  `GCPU10` double unsigned DEFAULT NULL,
  `GCPU10Peak` double unsigned DEFAULT NULL,
  `GCPU11` double unsigned DEFAULT NULL,
  `GCPU11Peak` double unsigned DEFAULT NULL,
  `GCPU12` double unsigned DEFAULT NULL,
  `GCPU12Peak` double unsigned DEFAULT NULL,
  `GCPU13` double unsigned DEFAULT NULL,
  `GCPU13Peak` double unsigned DEFAULT NULL,
  `GCPU14` double unsigned DEFAULT NULL,
  `GCPU14Peak` double unsigned DEFAULT NULL,
  `GCPU15` double unsigned DEFAULT NULL,
  `GCPU15Peak` double unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8353 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `globaltotal`
--

DROP TABLE IF EXISTS `globaltotal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `globaltotal` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `TimeStamp` timestamp(6) NULL DEFAULT NULL,
  `FileName` varchar(45) NOT NULL,
  `GCPU` double unsigned DEFAULT NULL,
  `GCPUPeak` double unsigned DEFAULT NULL,
  `GMA` int(11) unsigned DEFAULT NULL,
  `GMAPeak` int(11) unsigned DEFAULT NULL,
  `GPC` int(11) unsigned DEFAULT NULL,
  `GPCPeak` int(11) unsigned DEFAULT NULL,
  `GHC` int(11) unsigned DEFAULT NULL,
  `GHCPeak` int(11) unsigned DEFAULT NULL,
  `GHPF` double unsigned DEFAULT NULL,
  `GCPUP` double unsigned DEFAULT NULL,
  `GCPUPPeak` double unsigned DEFAULT NULL,
  `GMF` double unsigned DEFAULT NULL,
  `GMFPeak` double unsigned DEFAULT NULL,
  `GMCOMM` double unsigned DEFAULT NULL,
  `GMCOMMPeak` double unsigned DEFAULT NULL,
  `GML` int(11) unsigned DEFAULT NULL,
  `GMLPeak` int(11) unsigned DEFAULT NULL,
  `GPFC` double unsigned DEFAULT NULL,
  `GPFCPeak` double unsigned DEFAULT NULL,
  `GMC` double unsigned DEFAULT NULL,
  `GMCPeak` double unsigned DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1423 DEFAULT CHARSET=utf8 COMMENT='		';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mri_data`
--

DROP TABLE IF EXISTS `mri_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mri_data` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `TimeStamp` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  `FileName` varchar(45) NOT NULL,
  `Process_Name` varchar(255) NOT NULL,
  `Process_Id` int(11) unsigned NOT NULL,
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
  `PRIVPeak` double unsigned DEFAULT NULL,
  `VIRT` double unsigned DEFAULT NULL,
  `VIRTPeak` double unsigned DEFAULT NULL,
  `PFS` double unsigned DEFAULT NULL,
  `PFSPeak` double unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `time_fk_idx` (`TimeStamp`)
) ENGINE=InnoDB AUTO_INCREMENT=616655 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `process`
--

DROP TABLE IF EXISTS `process`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `process` (
  `process_id` int(100) unsigned NOT NULL AUTO_INCREMENT,
  `process_name` varchar(255) DEFAULT NULL,
  `process_name_id` int(11) unsigned DEFAULT NULL,
  PRIMARY KEY (`process_id`),
  UNIQUE KEY `process_name_UNIQUE` (`process_name`,`process_name_id`)
) ENGINE=InnoDB AUTO_INCREMENT=781 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `time`
--

DROP TABLE IF EXISTS `time`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `time` (
  `time_id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `timeStamp` timestamp(6) NULL DEFAULT NULL,
  PRIMARY KEY (`time_id`),
  UNIQUE KEY `timeStamp_UNIQUE` (`timeStamp`)
) ENGINE=InnoDB AUTO_INCREMENT=35610 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `trace_queries`
--

DROP TABLE IF EXISTS `trace_queries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `trace_queries` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `parameters` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-04-08 21:22:06
