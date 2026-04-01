package com.hcmute.reservation.service;

import java.time.LocalTime;

public interface ConfigProviderService {
    int getDurationMinutes();
    int getBufferMinutes();
    int getSoftLockMinutes();
    double getDepositPerGuest();
    int getGracePeriodMinutes();
    int getMaxCapacityOverflow();
    int getMaxMergeTables();
    String getOpeningTime();
    String getClosingTime();

    String getConfigValue(String key, String defaultValue);
}
