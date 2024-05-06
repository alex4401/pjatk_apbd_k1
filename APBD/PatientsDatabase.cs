using System.Data;
using System.Data.Common;
using APBD.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;

namespace APBD;

public class PatientsDatabase : IPatientsDatabase
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public PatientsDatabase(ISqlConnectionFactory connFactory)
    {
        _connectionFactory = connFactory;
    }

    public async Task<Medicament?> GetMedicamentInfoByIdAsync(int id)
    {
        await using SqlConnection conn = await _connectionFactory.CreateConnectionAsync();
        await using SqlCommand comm = conn.CreateCommand();

        comm.Connection = conn;
        comm.CommandText = @"
            SELECT
                [IdMedicament], [Name], [Description], [Type]
            FROM [Medicament]
            WHERE IdMedicament = @Id
        ";
        comm.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using SqlDataReader reader = await comm.ExecuteReaderAsync();
        if (!reader.HasRows || !(await reader.ReadAsync()))
        {
            return null;
        }

        return new Medicament
        {
            IdMedicament = reader.GetInt32("IdMedicament"),
            Name = reader.GetString("Name"),
            Description = reader.GetString("Description"),
            Type = reader.GetString("Type"),
        };
    }

    public async Task<IReadOnlyList<PrescriptionEntry>> GetPrescriptionsForMedicamentByIdAsync(int medId)
    {
        await using SqlConnection conn = await _connectionFactory.CreateConnectionAsync();
        await using SqlCommand comm = conn.CreateCommand();

        comm.Connection = conn;
        comm.CommandText = @"
            SELECT
                A.[IdPrescription], A.[Dose], A.[Details], P.[Date], P.[DueDate],
                P.[IdPatient], P.[IdDoctor]
            FROM [Prescription_Medicament] A
            INNER JOIN [Prescription] P ON P.[IdPrescription] = A.[IdPrescription]
            WHERE A.IdMedicament = @Id
            ORDER BY P.[Date] DESC
        ";
        comm.Parameters.Add("@Id", SqlDbType.Int).Value = medId;

        await using SqlDataReader reader = await comm.ExecuteReaderAsync();
        List<PrescriptionEntry> results = new();

        while (await reader.ReadAsync())
        {
            results.Add(new PrescriptionEntry
            {
                IdPrescription = reader.GetInt32("IdPrescription"),
                Date = reader.GetDateTime("Date"),
                DueDate = reader.GetDateTime("DueDate"),
                IdPatient = reader.GetInt32("IdPatient"),
                IdDoctor = reader.GetInt32("IdDoctor"),
                Dose = reader.GetInt32("Dose"),
                Details = reader.GetString("Details")
            });
        }

        return results;
    }

    public async Task<bool> CheckPatientExistsAsync(int id)
    {
        await using SqlConnection conn = await _connectionFactory.CreateConnectionAsync();
        await using SqlCommand comm = conn.CreateCommand();

        comm.Connection = conn;
        comm.CommandText = @"
            SELECT
                [IdPatient]
            FROM [Patient]
            WHERE IdPatient = @Id
        ";
        comm.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using SqlDataReader reader = await comm.ExecuteReaderAsync();
        return reader.HasRows;
    }

    public async Task<bool> DeletePatientInfoAsync(int id)
    {
        if (!(await CheckPatientExistsAsync(id)))
        {
            return false;
        }
        
        await using SqlConnection conn = await _connectionFactory.CreateConnectionAsync();
        
        SqlTransaction transaction = (SqlTransaction) await conn.BeginTransactionAsync();
#region Clear related records in [Prescription_Medicament]
        await using SqlCommand comm1 = conn.CreateCommand();
        comm1.Connection = conn;
        comm1.Transaction = transaction;
        comm1.CommandText = @"
            DELETE A FROM [Prescription_Medicament] A
            INNER JOIN [Prescription] P ON P.[IdPrescription] = A.[IdPrescription]
            WHERE P.[IdPatient] = @Id
        ";
        comm1.Parameters.Add("@Id", SqlDbType.Int).Value = id;
#endregion
#region Clear related records in [Prescription]
        await using SqlCommand comm2 = conn.CreateCommand();
        comm2.Connection = conn;
        comm2.Transaction = transaction;
        comm2.CommandText = @"
            DELETE FROM [Prescription]
            WHERE [IdPatient] = @Id
        ";
        comm2.Parameters.Add("@Id", SqlDbType.Int).Value = id;
#endregion
#region Clear record in [Patient]
        await using SqlCommand comm3 = conn.CreateCommand();
        comm3.Connection = conn;
        comm3.Transaction = transaction;
        comm3.CommandText = @"
            DELETE FROM [Patient]
            WHERE [IdPatient] = @Id
        ";
        comm3.Parameters.Add("@Id", SqlDbType.Int).Value = id;
#endregion

        await comm1.ExecuteNonQueryAsync();
        await comm2.ExecuteNonQueryAsync();
        await comm3.ExecuteNonQueryAsync();
        await transaction.CommitAsync();

        return true;
    }
}
