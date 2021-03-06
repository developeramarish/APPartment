﻿using APPartment.Data;
using APPartment.Enums;
using APPartment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APPartment.Utilities
{
    public class HistoryHtmlBuilder
    {
        private DataAccessContext context;

        public HistoryHtmlBuilder(DataAccessContext context)
        {
            this.context = context;
        }

        public string BuildLastUpdateBaseObjectHistoryForWidget(long objectId)
        {
            var result = string.Empty;

            var historyEvent = context.Histories.Where(x => x.ObjectId == objectId || x.TargetId == objectId).OrderByDescending(x => x.Id).FirstOrDefault();

            var historyEventString = new StringBuilder();
            var isSubObject = historyEvent.TargetId == null ? false : true;
            var subObjectTargetObject = new object();
            long subObjectType = 0;
            long parentObjectType = 0;

            if (isSubObject)
            {
                subObjectType = context.Objects.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault().ObjectTypeId;
            }
            else
            {
                if (historyEvent.FunctionTypeId != (int)HistoryFunctionTypes.Delete)
                {
                    if (context.Objects.Any(x => x.ObjectId == historyEvent.ObjectId))
                    {
                        parentObjectType = context.Objects.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault().ObjectTypeId;
                    }
                }
            }

            if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Create)
            {
                if (isSubObject)
                {
                    if (context.Objects.Any(x => x.ObjectId == historyEvent.TargetId))
                    {
                        var parentObjectTypeId = context.Objects.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault().ObjectTypeId;

                        switch (subObjectType)
                        {
                            case (int)ObjectTypes.Comment:
                                historyEventString.Append("Posted a <strong>comment</strong> in object ");

                                switch (parentObjectTypeId)
                                {
                                    case (int)ObjectTypes.Inventory:
                                        var theInventory = context.Inventories.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theInventory.ObjectId, theInventory.Name));
                                        break;
                                    case (int)ObjectTypes.Hygiene:
                                        var theHygiene = context.Hygienes.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theHygiene.ObjectId, theHygiene.Name));
                                        break;
                                    case (int)ObjectTypes.Issue:
                                        var theIssue = context.Issues.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theIssue.ObjectId, theIssue.Name));
                                        break;
                                }
                                break;
                            case (int)ObjectTypes.Image:
                                historyEventString.Append("Attached an <strong>image</strong> in object ");

                                switch (parentObjectTypeId)
                                {
                                    case (int)ObjectTypes.Inventory:
                                        var theInventory = context.Inventories.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theInventory.ObjectId, theInventory.Name));
                                        break;
                                    case (int)ObjectTypes.Hygiene:
                                        var theHygiene = context.Hygienes.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theHygiene.ObjectId, theHygiene.Name));
                                        break;
                                    case (int)ObjectTypes.Issue:
                                        var theIssue = context.Issues.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theIssue.ObjectId, theIssue.Name));
                                        break;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    historyEventString.Append("Created an object ");

                    switch (parentObjectType)
                    {
                        case (int)ObjectTypes.Inventory:
                            var theInventoryObject = context.Inventories.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name));
                            break;
                        case (int)ObjectTypes.Hygiene:
                            var theHygieneObject = context.Hygienes.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name));
                            break;
                        case (int)ObjectTypes.Issue:
                            var theIssueObject = context.Issues.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong>.", theIssueObject.ObjectId, theIssueObject.Name));
                            break;
                    }
                }
            }
            else if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Update)
            {
                if (isSubObject)
                {
                    if (subObjectType == (int)ObjectTypes.Comment)
                    {
                        // TODO: Implement this case when comments become editable.
                        // ATM we will only display parent objects history
                    }
                    else if (subObjectType == (int)ObjectTypes.Image)
                    {
                        // TODO: Implement this case if images become editable.
                        // ATM we will only display parent objects history
                    }
                }
                else
                {
                    switch (parentObjectType)
                    {
                        case (int)ObjectTypes.Inventory:
                            var theInventoryObject = context.Inventories.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                            if (historyEvent.ColumnName == "IsCompleted")
                            {
                                var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                historyEventString.Append("Marked an object ");

                                if (wasMarkedAsCompleted)
                                {
                                    historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>supplied</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name));
                                }
                                else
                                {
                                    historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>not supplied</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name));
                                }
                            }
                            else if (historyEvent.ColumnName == "Status")
                            {
                                historyEventString.Append("Set status as ");

                                switch (historyEvent.NewValue)
                                {
                                    case "1":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Inventory1));
                                        break;
                                    case "2":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Inventory2));
                                        break;
                                    case "3":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Inventory3));
                                        break;
                                    case "4":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Critical));
                                        break;
                                }
                            }
                            else
                            {
                                historyEventString.Append(string.Format("Updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theInventoryObject.Name, theInventoryObject.ObjectId));
                            }
                            break;
                        case (int)ObjectTypes.Hygiene:
                            var theHygieneObject = context.Hygienes.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                            if (historyEvent.ColumnName == "IsCompleted")
                            {
                                var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                historyEventString.Append("Marked an object ");

                                if (wasMarkedAsCompleted)
                                {
                                    historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>cleaned</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name));
                                }
                                else
                                {
                                    historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>due cleaning</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name));
                                }
                            }
                            else if (historyEvent.ColumnName == "Status")
                            {
                                historyEventString.Append("Set status as ");

                                switch (historyEvent.NewValue)
                                {
                                    case "1":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Hygiene1));
                                        break;
                                    case "2":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Hygiene2));
                                        break;
                                    case "3":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Hygiene3));
                                        break;
                                    case "4":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Critical));
                                        break;
                                }
                            }
                            else
                            {
                                historyEventString.Append(string.Format("Updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theHygieneObject.Name, theHygieneObject.ObjectId));
                            }
                            break;
                        case (int)ObjectTypes.Issue:
                            var theIssueObject = context.Issues.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                            if (historyEvent.ColumnName == "IsCompleted")
                            {
                                var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                historyEventString.Append("Marked an object ");

                                if (wasMarkedAsCompleted)
                                {
                                    historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>closed</strong>.", theIssueObject.ObjectId, theIssueObject.Name));
                                }
                                else
                                {
                                    historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>open</strong>.", theIssueObject.ObjectId, theIssueObject.Name));
                                }
                            }
                            else if (historyEvent.ColumnName == "Status")
                            {
                                historyEventString.Append("Set status as ");

                                switch (historyEvent.NewValue)
                                {
                                    case "1":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Issues1));
                                        break;
                                    case "2":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Issues2));
                                        break;
                                    case "3":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Issues3));
                                        break;
                                    case "4":
                                        historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Critical));
                                        break;
                                }
                            }
                            else
                            {
                                historyEventString.Append(string.Format("Updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theIssueObject.Name, theIssueObject.ObjectId));
                            }
                            break;
                    }
                }
            }

            result = historyEventString.ToString();

            return result;
        }

        public List<string> BuildBaseObjectHistory(List<History> history)
        {
            var result = new List<string>();

            foreach (var historyEvent in history.OrderByDescending(x => x.Id))
            {
                var historyEventString = new StringBuilder();
                var username = context.Users.Find(historyEvent.UserId).Username;
                var isSubObject = historyEvent.TargetId == null ? false : true;
                long objectType = 0;
                var when = historyEvent.When.ToString("dd'/'MM'/'yyyy HH:mm:ss");

                if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Delete)
                {
                    objectType = (long)historyEvent.DeletedObjectObjectType;
                }
                else
                {
                    if (context.Objects.Any(x => x.ObjectId == historyEvent.ObjectId))
                    {
                        objectType = context.Objects.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault().ObjectTypeId;
                    }
                }

                historyEventString.Append(username);

                if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Create)
                {
                    if (isSubObject)
                    {
                        if (objectType == (int)ObjectTypes.Comment)
                        {
                            historyEventString.Append(" posted a <strong>comment</strong>.");
                        }
                        else if (objectType == (int)ObjectTypes.Image)
                        {
                            historyEventString.Append(" attached an <strong>image</strong>.");
                        }
                    }
                    else
                    {
                        historyEventString.Append(" created this object.");
                    }
                }
                else if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Update)
                {
                    if (isSubObject)
                    {
                        if (objectType == (int)ObjectTypes.Comment)
                        {
                            // TODO: Implement this case when comments become editable.
                        }
                        else if (objectType == (int)ObjectTypes.Image)
                        {
                            // TODO: Implement this case if images become editable.
                        }
                    }
                    else
                    {
                        switch (objectType)
                        {
                            case (int)ObjectTypes.Inventory:
                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked as ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append("<strong>supplied</strong>.");
                                    }
                                    else
                                    {
                                        historyEventString.Append("<strong>not supplied</strong>.");
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Inventory1}</strong>.");
                                            break;
                                        case "2":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Inventory2}</strong>.");
                                            break;
                                        case "3":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Inventory3}</strong>.");
                                            break;
                                        case "4":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Critical}</strong>.");
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue));
                                }
                                break;
                            case (int)ObjectTypes.Hygiene:
                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked as ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append("<strong>cleaned</strong>.");
                                    }
                                    else
                                    {
                                        historyEventString.Append("<strong>due cleaning</strong>.");
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Hygiene1}</strong>.");
                                            break;
                                        case "2":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Hygiene2}</strong>.");
                                            break;
                                        case "3":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Hygiene3}</strong>.");
                                            break;
                                        case "4":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Critical}</strong>.");
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue));
                                }
                                break;
                            case (int)ObjectTypes.Issue:
                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked as ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append("<strong>closed</strong>.");
                                    }
                                    else
                                    {
                                        historyEventString.Append("<strong>open</strong>.");
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Issues1}</strong>.");
                                            break;
                                        case "2":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Issues2}</strong>.");
                                            break;
                                        case "3":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Issues3}</strong>.");
                                            break;
                                        case "4":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Critical}</strong>.");
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue));
                                }
                                break;
                            case (int)ObjectTypes.Survey:
                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked as ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append("<strong>completed</strong>.");
                                    }
                                    else
                                    {
                                        historyEventString.Append("<strong>pending</strong>.");
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Surveys1}</strong>.");
                                            break;
                                        case "2":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Surveys2}</strong>.");
                                            break;
                                        case "3":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Surveys3}</strong>.");
                                            break;
                                        case "4":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Critical}</strong>.");
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue));
                                }
                                break;
                            case (int)ObjectTypes.Chore:
                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked as ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append("<strong>completed</strong>.");
                                    }
                                    else
                                    {
                                        historyEventString.Append("<strong>pending</strong>.");
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Chores1}</strong>.");
                                            break;
                                        case "2":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Chores2}</strong>.");
                                            break;
                                        case "3":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Chores3}</strong>.");
                                            break;
                                        case "4":
                                            historyEventString.Append($"<strong>{BaseObjectStatus.Critical}</strong>.");
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue));
                                }
                                break;
                        }
                    }
                }
                else if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Delete)
                {
                    if (isSubObject)
                    {
                        historyEventString.Append(" <strong>deleted</strong> ");

                        if (objectType == (int)ObjectTypes.Comment)
                        {
                            historyEventString.Append("a comment.");
                        }
                        else if (objectType == (int)ObjectTypes.Image)
                        {
                            historyEventString.Append("an image.");
                        }
                    }
                }

                historyEventString.Append(string.Format(" <br/> <strong><span style=\"font-size: x-small;\">{0}</span></strong>", when));

                result.Add(historyEventString.ToString());
            }

            return result;
        }

        public List<string> BuildHomeHistory(List<History> history)
        {
            var result = new List<string>();

            foreach (var historyEvent in history.OrderByDescending(x => x.Id))
            {
                var historyEventString = new StringBuilder();
                var username = context.Users.Find(historyEvent.UserId).Username;
                var isSubObject = historyEvent.TargetId == null ? false : true;
                var subObjectTargetObject = new object();
                long subObjectType = 0;
                long parentObjectType = 0;
                var when = historyEvent.When.ToString("dd'/'MM'/'yyyy HH:mm:ss");

                if (isSubObject)
                {
                    subObjectType = context.Objects.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault().ObjectTypeId;
                }
                else
                {
                    if (historyEvent.FunctionTypeId != (int)HistoryFunctionTypes.Delete)
                    {
                        if (context.Objects.Any(x => x.ObjectId == historyEvent.ObjectId))
                        {
                            parentObjectType = context.Objects.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault().ObjectTypeId;
                        }
                    }
                }

                historyEventString.Append(username);

                if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Create)
                {
                    if (isSubObject)
                    {
                        if (context.Objects.Any(x => x.ObjectId == historyEvent.TargetId))
                        {
                            var parentObjectTypeId = context.Objects.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault().ObjectTypeId;

                            switch (subObjectType)
                            {
                                case (int)ObjectTypes.Comment:
                                    historyEventString.Append(" posted a <strong>comment</strong> in object ");

                                    switch (parentObjectTypeId)
                                    {
                                        case (int)ObjectTypes.Inventory:
                                            var theInventory = context.Inventories.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventory.ObjectId, theInventory.Name));
                                            break;
                                        case (int)ObjectTypes.Hygiene:
                                            var theHygiene = context.Hygienes.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygiene.ObjectId, theHygiene.Name));
                                            break;
                                        case (int)ObjectTypes.Issue:
                                            var theIssue = context.Issues.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssue.ObjectId, theIssue.Name));
                                            break;
                                        case (int)ObjectTypes.Survey:
                                            var theSurvey = context.Surveys.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurvey.ObjectId, theSurvey.Name));
                                            break;
                                        case (int)ObjectTypes.Chore:
                                            var theChore = context.Chores.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChore.ObjectId, theChore.Name));
                                            break;
                                    }
                                    break;
                                case (int)ObjectTypes.Image:
                                    historyEventString.Append(" attached an <strong>image</strong> in object ");

                                    switch (parentObjectTypeId)
                                    {
                                        case (int)ObjectTypes.Inventory:
                                            var theInventory = context.Inventories.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventory.ObjectId, theInventory.Name));
                                            break;
                                        case (int)ObjectTypes.Hygiene:
                                            var theHygiene = context.Hygienes.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygiene.ObjectId, theHygiene.Name));
                                            break;
                                        case (int)ObjectTypes.Issue:
                                            var theIssue = context.Issues.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssue.ObjectId, theIssue.Name));
                                            break;
                                        case (int)ObjectTypes.Survey:
                                            var theSurvey = context.Surveys.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurvey.ObjectId, theSurvey.Name));
                                            break;
                                        case (int)ObjectTypes.Chore:
                                            var theChore = context.Chores.Where(x => x.ObjectId == historyEvent.TargetId).FirstOrDefault();

                                            historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChore.ObjectId, theChore.Name));
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (parentObjectType)
                        {
                            case (int)ObjectTypes.Inventory:
                                var theInventoryObject = context.Inventories.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                historyEventString.Append(string.Format(" created an object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name));
                                break;
                            case (int)ObjectTypes.Hygiene:
                                var theHygieneObject = context.Hygienes.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                historyEventString.Append(string.Format(" created an object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name));
                                break;
                            case (int)ObjectTypes.Issue:
                                var theIssueObject = context.Issues.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                historyEventString.Append(string.Format(" created an object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name));
                                break;
                            case (int)ObjectTypes.Survey:
                                var theSurveyObject = context.Surveys.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                historyEventString.Append(string.Format(" created an object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name));
                                break;
                            case (int)ObjectTypes.Chore:
                                var theChoreObject = context.Chores.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                historyEventString.Append(string.Format(" created an object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name));
                                break;
                            case (int)ObjectTypes.HouseStatus:
                                if (historyEvent.ColumnName == "Status")
                                {
                                    var houseStatus = int.Parse(historyEvent.NewValue);
                                    var statusString = string.Empty;

                                    if (houseStatus == (int)HomeStatus.Green)
                                    {
                                        statusString = "<strong>free to enter</strong>";
                                    }
                                    else if (houseStatus == (int)HomeStatus.Yellow)
                                    {
                                        statusString = "<strong>enter catiously</strong>";
                                    }
                                    else if (houseStatus == (int)HomeStatus.Red)
                                    {
                                        statusString = "<strong>do not enter</strong>";
                                    }

                                    historyEventString.Append(string.Format(" set home status as {0}.", statusString));
                                }
                                break;
                        }
                    }
                }
                else if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Update)
                {
                    if (isSubObject)
                    {
                        if (subObjectType == (int)ObjectTypes.Comment)
                        {
                            // TODO: Implement this case when comments become editable.
                            // ATM we will only display parent objects history
                        }
                        else if (subObjectType == (int)ObjectTypes.Image)
                        {
                            // TODO: Implement this case if images become editable.
                            // ATM we will only display parent objects history
                        }
                    }
                    else
                    {
                        switch (parentObjectType)
                        {
                            case (int)ObjectTypes.House:
                                var theHomeObject = context.Houses.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                if (historyEvent.ColumnName == "Name")
                                {
                                    historyEventString.Append(string.Format(" updated this home's name. <br/> Previous value: <span style=\"text-decoration: line-through\">{0}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{1}</span>"
                                    , historyEvent.OldValue, historyEvent.NewValue));
                                }
                                break;
                            case (int)ObjectTypes.HouseStatus:
                                if (historyEvent.ColumnName == "Status")
                                {
                                    var houseStatus = int.Parse(historyEvent.NewValue);
                                    var statusString = string.Empty;

                                    if (houseStatus == (int)HomeStatus.Green)
                                    {
                                        statusString = "<strong>free to enter</strong>";
                                    }
                                    else if (houseStatus == (int)HomeStatus.Yellow)
                                    {
                                        statusString = "<strong>enter catiously</strong>";
                                    }
                                    else if (houseStatus == (int)HomeStatus.Red)
                                    {
                                        statusString = "<strong>do not enter</strong>";
                                    }

                                    historyEventString.Append(string.Format(" set home status as {0}.", statusString));
                                }
                                break;
                            case (int)ObjectTypes.Inventory:
                                var theInventoryObject = context.Inventories.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked an object ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>supplied</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name));
                                    }
                                    else
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>not supplied</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name));
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Inventory1));
                                            break;
                                        case "2":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Inventory2));
                                            break;
                                        case "3":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Inventory3));
                                            break;
                                        case "4":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", theInventoryObject.ObjectId, theInventoryObject.Name, BaseObjectStatus.Critical));
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong> in <a href='/Inventory/Index'>Inventory</a>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theInventoryObject.Name, theInventoryObject.ObjectId));
                                }
                                break;
                            case (int)ObjectTypes.Hygiene:
                                var theHygieneObject = context.Hygienes.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked an object ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>cleaned</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name));
                                    }
                                    else
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>due cleaning</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name));
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Hygiene1));
                                            break;
                                        case "2":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Hygiene2));
                                            break;
                                        case "3":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Hygiene3));
                                            break;
                                        case "4":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", theHygieneObject.ObjectId, theHygieneObject.Name, BaseObjectStatus.Critical));
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theHygieneObject.Name, theHygieneObject.ObjectId));
                                }
                                break;
                            case (int)ObjectTypes.Issue:
                                var theIssueObject = context.Issues.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked an object ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>closed</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name));
                                    }
                                    else
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>open</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name));
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Issues1));
                                            break;
                                        case "2":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Issues2));
                                            break;
                                        case "3":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Issues3));
                                            break;
                                        case "4":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Issues/Index'>Issues</a>.", theIssueObject.ObjectId, theIssueObject.Name, BaseObjectStatus.Critical));
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong> in <a href='/Issues/Index'>Issues</a>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theIssueObject.Name, theIssueObject.ObjectId));
                                }
                                break;
                            case (int)ObjectTypes.Survey:
                                var theSurveyObject = context.Surveys.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked an object ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>completed</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name));
                                    }
                                    else
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>pending</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name));
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name, BaseObjectStatus.Surveys1));
                                            break;
                                        case "2":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name, BaseObjectStatus.Surveys2));
                                            break;
                                        case "3":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name, BaseObjectStatus.Surveys3));
                                            break;
                                        case "4":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", theSurveyObject.ObjectId, theSurveyObject.Name, BaseObjectStatus.Critical));
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong> in <a href='/Surveys/Index'>Surveys</a>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theSurveyObject.Name, theSurveyObject.ObjectId));
                                }
                                break;
                            case (int)ObjectTypes.Chore:
                                var theChoreObject = context.Chores.Where(x => x.ObjectId == historyEvent.ObjectId).FirstOrDefault();

                                if (historyEvent.ColumnName == "IsCompleted")
                                {
                                    var wasMarkedAsCompleted = bool.Parse(historyEvent.NewValue);

                                    historyEventString.Append(" marked an object ");

                                    if (wasMarkedAsCompleted)
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>completed</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name));
                                    }
                                    else
                                    {
                                        historyEventString.Append(string.Format("<strong>[ID: {0}, Name: {1}]</strong> as <strong>pending</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name));
                                    }
                                }
                                else if (historyEvent.ColumnName == "Status")
                                {
                                    historyEventString.Append(" set status as ");

                                    switch (historyEvent.NewValue)
                                    {
                                        case "1":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name, BaseObjectStatus.Chores1));
                                            break;
                                        case "2":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name, BaseObjectStatus.Chores2));
                                            break;
                                        case "3":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name, BaseObjectStatus.Chores3));
                                            break;
                                        case "4":
                                            historyEventString.Append(string.Format("<strong>{2}</strong> for object <strong>[ID: {0}, Name: {1}]</strong> in <a href='/Chores/Index'>Chores</a>.", theChoreObject.ObjectId, theChoreObject.Name, BaseObjectStatus.Critical));
                                            break;
                                    }
                                }
                                else
                                {
                                    historyEventString.Append(string.Format(" updated {0} column in object <strong>[ID: {4}, Name: {3}]</strong> in <a href='/Chores/Index'>Chores</a>. <br/> Previous value: <span style=\"text-decoration: line-through\">{1}</span> <br/> <strong>Current value</strong>: <span style=\"background-color: #90EE90\">{2}</span>"
                                    , historyEvent.ColumnName.ToLower(), historyEvent.OldValue, historyEvent.NewValue, theChoreObject.Name, theChoreObject.ObjectId));
                                }
                                break;
                        }
                    }
                }
                else if (historyEvent.FunctionTypeId == (int)HistoryFunctionTypes.Delete)
                {
                    historyEventString.Append(" <strong>deleted</strong> an object ");

                    switch ((int)historyEvent.DeletedObjectObjectType)
                    {
                        case (int)ObjectTypes.Inventory:
                            historyEventString.Append(string.Format("<strong>[ID: {0}]</strong> in <a href='/Inventory/Index'>Inventory</a>.", historyEvent.ObjectId));
                            break;
                        case (int)ObjectTypes.Hygiene:
                            historyEventString.Append(string.Format("<strong>[ID: {0}]</strong> in <a href='/Hygiene/Index'>Hygiene</a>.", historyEvent.ObjectId));
                            break;
                        case (int)ObjectTypes.Issue:
                            historyEventString.Append(string.Format("<strong>[ID: {0}]</strong> in <a href='/Issues/Index'>Issues</a>.", historyEvent.ObjectId));
                            break;
                        case (int)ObjectTypes.Survey:
                            historyEventString.Append(string.Format("<strong>[ID: {0}]</strong> in <a href='/Surveys/Index'>Surveys</a>.", historyEvent.ObjectId));
                            break;
                        case (int)ObjectTypes.Chore:
                            historyEventString.Append(string.Format("<strong>[ID: {0}]</strong> in <a href='/Chores/Index'>Chores</a>.", historyEvent.ObjectId));
                            break;
                    }
                }

                // Only implemented cases get added to the results list
                if (historyEventString.ToString().Length == username.Length)
                {
                    // The case is not implemented...
                }
                else
                {
                    historyEventString.Append(string.Format(" <br/> <strong><span style=\"font-size: x-small;\">{0}</span></strong>", when));

                    result.Add(historyEventString.ToString());
                }
            }

            return result;
        }
    }
}
