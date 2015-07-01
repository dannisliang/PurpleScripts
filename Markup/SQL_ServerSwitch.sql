-- MySQL Script generated by MySQL Workbench
-- Mon Jun  8 14:32:07 2015
-- Model: New Model    Version: 1.0
SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema PurpleDatabase
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `PurpleDatabase` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci ;
USE `PurpleDatabase` ;

-- -----------------------------------------------------
-- Table `PurpleDatabase`.`server_switch`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `PurpleDatabase`.`server_switch` ;

CREATE TABLE IF NOT EXISTS `PurpleDatabase`.`server_switch` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `from_server` INT NOT NULL,
  `to_server` INT NOT NULL,
  `account_id` INT NOT NULL,
  `token` VARCHAR(45) NOT NULL,
  `used` TINYINT(1) NOT NULL DEFAULT 0,
  `timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `to_server_id1` (`to_server` ASC),
  INDEX `from_server_id1` (`from_server` ASC),
  INDEX `account_id_id3` (`account_id` ASC),
  CONSTRAINT `from_server_1`
    FOREIGN KEY (`from_server`)
    REFERENCES `PurpleDatabase`.`server` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `to_server_1`
    FOREIGN KEY (`to_server`)
    REFERENCES `PurpleDatabase`.`server` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `account_id_1`
    FOREIGN KEY (`account_id`)
    REFERENCES `PurpleDatabase`.`account` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;