using System;
using System.Collections.Generic;
using affolterNET.Data.Interfaces;
using FluentAssertions;

namespace affolterNET.Data.TestHelpers.Helpers
{
    public class AssertHelper
    {
        public readonly DbOperations DbOps;

        private readonly IDtoFactory dtoFactory;

        private IDictionary<string, object> paramsDict = new Dictionary<string, object>();

        public AssertHelper(DbOperations dbOps, IDtoFactory dtoFactory)
        {
            DbOps = dbOps;
            this.dtoFactory = dtoFactory;
        }

        public bool InputChecked { get; set; }

        public void SetParams(IDictionary<string, object> pd)
        {
            paramsDict = pd;
        }

        public T GetById<T>(object id, bool mustExist = true, string idName = "Id")
            where T : IDtoBase
        {
            var item = DbOps.SelectById<T>(id, idName);
            var tableName = item.GetTableName();
            if (mustExist)
            {
                item.Should().NotBeNull($"Eintrag in {tableName} nicht gefunden");
            }

            return item;
        }

        // public void CheckErstelltLetzteAenderung(IDto check, bool istUpdate, bool istAktiv = true)
        // {
        // var dbUserName = Thread.CurrentPrincipal.Identity.GetAngemeldeterBenutzerString();
        // Assert.AreEqual(check.IstAktiv, istAktiv, "Gelöscht-Zustand ist falsch");
        // if (istUpdate)
        // {
        // this.CheckDateMitToleranz(check.LetzteAenderungAm, DateTime.Now);
        // Assert.AreEqual(
        // check.LetzteAenderungDurch,
        // $"{dbUserName}",
        // "LetzteAenderungDurch darf nicht NULL sein bei UPDATE");
        // }
        // else
        // {
        // this.CheckDateMitToleranz(check.LetzteAenderungAm);
        // Assert.AreEqual(check.LetzteAenderungDurch, null, "LetzteAenderungDurch muss NULL sein bei INSERT");
        // this.CheckDateMitToleranz(check.ErstelltAm, DateTime.Now);
        // Assert.AreEqual(check.ErstelltDurch, $"{dbUserName}");
        // }
        // }

        public void CheckDate(DateTime? toCheck, DateTime? toCompare = null)
        {
            if (toCompare == null)
            {
                toCheck.Should().BeNull("Das Datum müsste NULL sein");
            }
            else
            {
                toCheck.HasValue.Should().BeTrue("Check Datum darf nicht NULL sein");

                // ReSharper disable once PossibleInvalidOperationException
                toCompare.Value.Should().BeCloseTo(
                    toCheck.Value,
                    TimeSpan.FromSeconds(1),
                    "Das Datum ist mehr als 1 Sekunden daneben");
            }
        }

        public void CheckDateMitToleranz(DateTime? toCheck, DateTime? toCompare = null)
        {
            if (toCompare == null)
            {
                toCheck.Should().BeNull("Das Datum müsste NULL sein");
            }
            else
            {
                toCheck.HasValue.Should().BeTrue("Check Datum darf nicht NULL sein");

                // ReSharper disable once PossibleInvalidOperationException
                toCompare.Value.Should().BeCloseTo(
                    toCheck.Value,
                    TimeSpan.FromSeconds(30),
                    "Das Datum ist mehr als 30 Sekunden daneben");
            }
        }

        public T GetParam<T>(string name)
        {
            paramsDict.ContainsKey(name).Should().BeTrue("ParamsDict muss vorhanden sein");
            try
            {
                return (T)Convert.ChangeType(paramsDict[name], typeof(T));
            }
            catch (InvalidCastException ex)
            {
                throw new Exception($"Falscher Typ für Parameter {name}", ex);
            }
        }

        public void CheckInsertParams<TDto>(params string[] paras)
            where TDto : IDtoBase
        {
            var item = dtoFactory.Get<TDto>();
            var date = item.GetInsertedDateName();
            var user = item.GetInsertedUserName();

            paramsDict.ContainsKey(date).Should().BeTrue($"{date} nicht gefunden auf InsertParams");
            paramsDict.ContainsKey(user).Should().BeTrue($"{user} nicht gefunden auf InsertParams");
            CheckQueryParams(paras);
        }

        public void CheckUpdateParams<TDto>(params string[] paras)
            where TDto : IDtoBase
        {
            var item = dtoFactory.Get<TDto>();
            var date = item.GetUpdatedDateName();
            var user = item.GetUpdatedUserName();
            paramsDict.ContainsKey(date).Should().BeTrue($"{date} nicht gefunden auf UpdateParams");
            paramsDict.ContainsKey(user).Should().BeTrue($"{user} nicht gefunden auf UpdateParams");
            CheckQueryParams(paras);
        }

        public void CheckQueryParams(params string[] paras)
        {
            foreach (var key in paras)
            {
                paramsDict.ContainsKey(key).Should()
                    .BeTrue($"Parameter {key} wurde nicht oder nicht unter diesem Namen abgelegt.");
            }

            InputChecked = true;
        }
    }
}