using System.Collections;
using System.Collections.Generic;
using System.Data;

public class KillerDeadTable {

    private DataTable dTable;

    public KillerDeadTable()
    {
        dTable = new DataTable("killerKilledTable");
        dTable.Columns.Add("idKilled", typeof(int));
        dTable.Columns.Add("idKiller", typeof(int));
        dTable.Columns.Add("deathTiming", typeof(float));
        dTable.Columns.Add("wasLastLife", typeof(bool));

    }

    /// <summary>
    /// Add a kill to the kill table
    /// </summary>
    /// <param name="idKilled">player killed</param>
    /// <param name="idKiller">player who killed</param>
    /// <param name="deathTiming">Death Game Time</param>
    /// <param name="wasLastLife">Was the last life</param>
    public void AddKill (int idKilled, int idKiller, float deathTiming, bool wasLastLife)
    {
        dTable.Rows.Add(idKilled, idKiller, deathTiming, wasLastLife);
    }

    /// <summary>
    /// Get the current number of death of the specified player
    /// </summary>
    /// <param name="idKilled">the player id</param>
    /// <returns>the current death number of the specified player id</returns>
    public int GetNbDeathOf(int idKilled)
    {
        return dTable.Select("idKilled = " + idKilled).Length;
    }

    /// <summary>
    /// Get the current number of kills of the specified player
    /// </summary>
    /// <param name="idKiller">the player id</param>
    /// <returns>the current kill number of the specified player id</returns>
    public int GetNbKillOf(int idKiller)
    {
        return dTable.Select("idKiller = " + idKiller).Length;
    }

    /// <summary>
    /// Get the final death list, sorted from first to last
    /// </summary>
    /// <returns>the final death list, sorted from first to last</returns>
    public int[] GetDeathOrder()
    {
        List<int> _retList = new List<int>();
        DataRow[] r = dTable.Select("wasLastLife = true", "deathTiming ASC");
        for(int i = 0; i < r.Length; i++)
        {
            _retList.Add((int) r[i]["idKilled"]);
        }
        return _retList.ToArray();
    }

    /// <summary>
    /// Get an array with all the killers of the player
    /// </summary>
    /// <param name="idKilled">the player id</param>
    /// <returns>Array of all the killer of the specified player id</returns>
    public int[] GetKillersOf(int idKilled)
    {
        DataRow[] r = dTable.Select("idKilled = "+idKilled);
        List<int> retList = new List<int>();

        foreach(DataRow row in r)
        {
            retList.Add((int)row["idKiller"]);
        }

        return retList.ToArray();
    }
    
    /// <summary>
    /// Get all killers if the specified id, with Ony one reference of each
    /// </summary>
    /// <param name="idKilled">the player id</param>
    /// <returns>Array of all unique killers of the specified player id</returns>
    public int[] GetUniqueKillersOf(int idKilled)
    {
        int[] _tempTable = GetKillersOf(idKilled);
        List<int> retList = new List<int>();

        foreach(int i in _tempTable)
        {
            if(!retList.Contains(i))
                retList.Add(i);
        }

        return retList.ToArray();
    }

    /// <summary>
    /// Get the numbers of time the Killed player was by the Killer player
    /// </summary>
    /// <param name="idKilled">The specified killed player</param>
    /// <param name="idKiller">The specified killer player</param>
    /// <returns>Numbers of time the Killed player was by the Killer player</returns>
    public int GetNbKillsFrom(int idKilled, int idKiller)
    {
        DataRow[] r = dTable.Select("idKilled = " + idKilled + " AND idKiller = " + idKiller);
        return r.Length;
    }

    /// <summary>
    /// Get the biggest killer of the player
    /// </summary>
    /// <param name="idKilled">the player killed</param>
    /// <returns>the id of the biggest killer</returns>
    public int GetNemesis(int idKilled) //<<<< return int array for equality
    {
        int[] _killersArray = GetKillersOf(idKilled);
        List<int> testedIds = new List<int>();
        int _rValue = 0;
        int _maxKill = 0;

        foreach(int i in _killersArray)
        {
            if(!testedIds.Contains(i))
            {
                if(GetNbKillsFrom(idKilled, i) > _maxKill)
                {
                    _rValue = i;
                    _maxKill = GetNbKillsFrom(idKilled, i);

                }
                testedIds.Add(i);
            }
        }
        return _rValue;
    }
}
