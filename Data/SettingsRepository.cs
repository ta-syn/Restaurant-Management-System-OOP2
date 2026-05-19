using Microsoft.Data.Sqlite;
using System;

namespace RestaurantManagementSystem.Data;

public static class SettingsRepository
{
    public static double GetTaxRate()
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SettingValue FROM SystemSettings WHERE SettingKey = 'TaxRate';";
            var result = cmd.ExecuteScalar();
            if (result != null && double.TryParse(result.ToString(), out double rate))
            {
                return rate;
            }
        }
        catch
        {
            // If table doesn't exist yet or other DB error, default to 10.0
        }
        return 10.0;
    }

    public static void SetTaxRate(double rate)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            
            // Ensure settings table exists (just in case)
            var createCmd = connection.CreateCommand();
            createCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SystemSettings (
                    SettingKey TEXT PRIMARY KEY,
                    SettingValue TEXT NOT NULL
                );
            ";
            createCmd.ExecuteNonQuery();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT OR REPLACE INTO SystemSettings (SettingKey, SettingValue)
                VALUES ('TaxRate', @val);
            ";
            cmd.Parameters.AddWithValue("@val", rate.ToString("F2"));
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SettingsRepository] Error saving TaxRate: {ex.Message}");
        }
    }
}
