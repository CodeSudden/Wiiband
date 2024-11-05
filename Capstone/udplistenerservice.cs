using MySqlConnector;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Capstone
{
    public class UdpListenerService : BackgroundService
    {
        private const int listenPort = 6061;
        private const int espClientPort = 12345;
        private readonly string _connectionString;
        private readonly ILogger<UdpListenerService> _logger;

        public UdpListenerService(ILogger<UdpListenerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            // Retrieve the connection string from appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            logger.LogInformation(_connectionString);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            _logger.LogInformation("UDP Listener Service started, waiting for broadcast...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (listener.Available > 0)
                    {
                        byte[] bytes = listener.Receive(ref groupEP);
                        string receivedData = Encoding.ASCII.GetString(bytes);
                        _logger.LogInformation($"Received broadcast from {groupEP}: {receivedData}");

                        if (receivedData.StartsWith("CLIENT,"))
                        {
                            ProcessClientData(receivedData, groupEP.Address.ToString());
                        }
                        else if (receivedData.Contains("SCANNER"))
                        {
                            ProcessScannerData(receivedData, groupEP.Address.ToString());
                     
                        }

                    }
                    await Task.Delay(10, stoppingToken);  // Avoids CPU overload
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in UDP Listener");
                }
            }

            listener.Close();
            _logger.LogInformation("UDP Listener Service stopping.");
        }

        private void ProcessClientData(string data, string clientIPAddress)
        {
            string[] parts = data.Split(',');
            if (parts.Length == 3)
            {
                string rfidTag = parts[1];
                string ipAddress = parts[2];
                SaveOrUpdateMasterRecord(rfidTag, ipAddress, clientIPAddress);
            }
            else
            {
                _logger.LogWarning("Invalid CLIENT data format.");
            }
        }


        private void ProcessScannerData(string data, string clientIPAddress)
        {
           
            string[] parts = data.Split(',');
            if (parts.Length == 2)
            {
                string scanner = parts[0];
                string rfidTag = parts[1];
                SaveToDatabase(rfidTag, clientIPAddress, scanner);
                SaveScannerLog(rfidTag, clientIPAddress, scanner);
            }
            else
            {
                _logger.LogWarning("Invalid CLIENT data format.");
            }
        }

        private void SaveOrUpdateMasterRecord(string rfidTag, string ipAddress, string clientIPAddress)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string selectQuery = "SELECT COUNT(*) FROM wiibandmaster WHERE TAGID = @rfidTag";
            string insertQuery = "INSERT INTO wiibandmaster (TAGID, IPADDRESS) VALUES (@rfidTag, @ipAddress)";
            string updateQuery = "UPDATE wiibandmaster SET IPADDRESS = @ipAddress WHERE TAGID = @rfidTag";

            SqlCommand selectCmd = new SqlCommand(selectQuery, connection);
            selectCmd.Parameters.AddWithValue("@rfidTag", rfidTag);

            try
            {
                connection.Open();
                int count = (int)selectCmd.ExecuteScalar();
                string statusMessage;

                if (count > 0)
                {
                    SqlCommand updateCmd = new SqlCommand(updateQuery, connection);
                    updateCmd.Parameters.AddWithValue("@rfidTag", rfidTag);
                    updateCmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                    updateCmd.ExecuteNonQuery();
                    _logger.LogInformation($"Updated IP address for RFTAG {rfidTag}.");
                    statusMessage = $"{rfidTag},\"CLIENT with tag ID:{rfidTag} and IP:{ipAddress} is UPDATED\"";
                }
                else
                {
                    SqlCommand insertCmd = new SqlCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@rfidTag", rfidTag);
                    insertCmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                    insertCmd.ExecuteNonQuery();
                    _logger.LogInformation($"Inserted new RFTAG {rfidTag} with IP address {ipAddress}.");
                    statusMessage = $"{rfidTag},\"CLIENT with tag ID:{rfidTag} and IP:{ipAddress} is now registered\"";
                }

                SendStatusToClient(statusMessage, clientIPAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in SaveOrUpdateMasterRecord");
            }
        }

        private void SendStatusToClient(string message, string clientIPAddress)
        {
            using UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse(clientIPAddress), espClientPort);
                byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                udpClient.Send(sendBytes, sendBytes.Length, clientEP);
                _logger.LogInformation($"Sent status message to ESP client {clientIPAddress}:{espClientPort}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending UDP message");
            }
        }

        //private void SaveToDatabase(string tagId, string ipAddress, string scannerLoc)
        //{
        //    using SqlConnection connection = new SqlConnection(_connectionString);
        //    connection.Open();

        //    if (scannerLoc == "SCANNERCOUNTER")
        //    {
        //        // Check if there is an entry without an endtime
        //        string checkQuery = @"
        //    SELECT COUNT(*) FROM wiibandmonitor 
        //    WHERE wiibandtag = @tagId AND endtime IS NULL";

        //        using SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
        //        checkCmd.Parameters.AddWithValue("@tagId", tagId);
        //        int openEntryCount = (int)checkCmd.ExecuteScalar();

        //        // If there's no open entry, insert a new record
        //        if (openEntryCount == 0)
        //        {
        //            string insertQuery = @"
        //        INSERT INTO wiibandmonitor (wiibandtag, wiibandip, starttime)
        //        VALUES (@tagId, @ipAddress, @starttime)";

        //            using SqlCommand insertCmd = new SqlCommand(insertQuery, connection);
        //            insertCmd.Parameters.AddWithValue("@tagId", tagId);
        //            insertCmd.Parameters.AddWithValue("@ipAddress", ipAddress);
        //            insertCmd.Parameters.AddWithValue("@starttime", DateTime.Now);


        //            insertCmd.ExecuteNonQuery();
        //            _logger.LogInformation("Inserted new entry with starttime for SCANNERCOUNTER.");
        //        }
        //    }
        //    else if (scannerLoc == "SCANNEREXIT")
        //    {
        //        // Update the latest entry's endtime if it exists
        //        string updateQuery = @"
        //    UPDATE wiibandmonitor
        //    SET endtime = @endtime
        //    WHERE wiibandtag = @tagId AND endtime IS NULL";

        //        using SqlCommand updateCmd = new SqlCommand(updateQuery, connection);
        //        updateCmd.Parameters.AddWithValue("@tagId", tagId);
        //        updateCmd.Parameters.AddWithValue("@endtime", DateTime.Now);

        //        int rowsAffected = updateCmd.ExecuteNonQuery();
        //        if (rowsAffected > 0)
        //        {
        //            _logger.LogInformation("Updated endtime for SCANNEREXIT.");
        //        }
        //        else
        //        {
        //            _logger.LogWarning("No open entry to update for SCANNEREXIT.");
        //        }
        //    }
        //}
        private void SaveToDatabase(string tagId, string ipAddress, string scannerLoc)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            if (scannerLoc == "SCANNERCOUNTER")
            {
                // Update the StartTime for the Dashboard table if SCANNERCOUNTER is detected
                string updateStartTimeQuery = @"
            UPDATE wiijump_db.dbo.Dashboard
            SET StartTime = @startTime
            WHERE WiibandID = @tagId AND StartTime IS NULL";

                using SqlCommand updateStartCmd = new SqlCommand(updateStartTimeQuery, connection);
                updateStartCmd.Parameters.AddWithValue("@tagId", tagId);
                updateStartCmd.Parameters.AddWithValue("@startTime", DateTime.Now);

                int rowsAffected = updateStartCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("StartTime updated for SCANNERCOUNTER.");
                }
                else
                {
                    _logger.LogWarning("No entry found to update StartTime for SCANNERCOUNTER.");
                }
            }
            else if (scannerLoc == "SCANNEREXIT")
            {
                // Update the EndTime for the Dashboard table if SCANNEREXIT is detected
                string updateEndTimeQuery = @"
            UPDATE wiijump_db.dbo.Dashboard
            SET EndTime = @endTime
            WHERE WiibandID = @tagId AND EndTime IS NULL";

                using SqlCommand updateEndCmd = new SqlCommand(updateEndTimeQuery, connection);
                updateEndCmd.Parameters.AddWithValue("@tagId", tagId);
                updateEndCmd.Parameters.AddWithValue("@endTime", DateTime.Now);

                int rowsAffected = updateEndCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("EndTime updated for SCANNEREXIT.");
                }
                else
                {
                    _logger.LogWarning("No entry found to update EndTime for SCANNEREXIT.");
                }
            }
        }

        private void SaveScannerLog(string tagId, string ipAddress, string scannerLoc)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query = @"
        INSERT INTO wiibandtag (TAGID, IPADDRESS, DATETIME, SCANNERLOC)
        VALUES (@tagId, @ipAddress, @dateTime, @scannerLoc);";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@tagId", tagId);
            cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
            cmd.Parameters.AddWithValue("@dateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@scannerLoc", scannerLoc);

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                _logger.LogInformation("Data saved to database (inserted or updated).");

                // Send UDP commands to the client based on rfidTag
                SendUdpCommandToClient(tagId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveToDatabase");
            }
        }

        private void SendUdpCommandToClient(string rfidTag)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT IPADDRESS FROM wiibandmaster WHERE TAGID = @rfidTag";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@rfidTag", rfidTag.Trim());

            try
            {
                connection.Open();
                string clientIPAddress = cmd.ExecuteScalar()?.ToString();

                if (!string.IsNullOrEmpty(clientIPAddress))
                {
                    SendUdpMessage(clientIPAddress, "17 E7 29 66,BUZZER,500,2");
                    SendUdpMessage(clientIPAddress, "17 E7 29 66,BLUEON");
                    _logger.LogInformation($"Sent UDP messages to {clientIPAddress} for RFID Tag {rfidTag}.");
                }
                else
                {
                    _logger.LogWarning($"No IP address found for RFID Tag {rfidTag} in wiibandmaster.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendUdpCommandToClient");
            }
        }

        private void SendUdpMessage(string ipAddress, string message)
        {
            using UdpClient udpClient = new UdpClient();
            try
            {
                IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse(ipAddress), espClientPort);
                byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                udpClient.Send(sendBytes, sendBytes.Length, clientEP);
                _logger.LogInformation($"Sent UDP message '{message}' to {ipAddress}:{espClientPort}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending UDP message to {ipAddress}");
            }
        }
    }

}
