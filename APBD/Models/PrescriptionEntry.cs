namespace APBD.Models;

public class PrescriptionEntry
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
    public int Dose { get; set; }
    public string Details { get; set; }
}