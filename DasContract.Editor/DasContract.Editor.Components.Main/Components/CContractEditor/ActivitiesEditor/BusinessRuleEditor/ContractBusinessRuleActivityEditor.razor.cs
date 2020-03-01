﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bonsai.RazorComponents.MaterialBootstrap.Components.CAlert;
using Bonsai.RazorComponents.MaterialBootstrap.Components.CDialogWindow;
using Bonsai.Utils.String;
using DasContract.Editor.Entities;
using DasContract.Editor.Entities.Integrity.Analysis;
using DasContract.Editor.Entities.Integrity.Contract.Processes;
using DasContract.Editor.Entities.Integrity.Contract.Processes.Process.Activities;
using DasContract.Editor.Entities.Processes.Diagrams;
using DasContract.Editor.Entities.Processes.Process.Activities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DasContract.Editor.Components.Main.Components.CContractEditor.ActivitiesEditor.BusinessRuleEditor
{
    public partial class ContractBusinessRuleActivityEditor : LoadableComponent
    {
        [Parameter]
        public EditorContract Contract { get; set; }

        [Parameter]
        public ContractBusinessRuleActivity BusinessActivity { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public string DiagramXML => BusinessActivity.Diagram.DiagramXML;

        protected ContractBusinessRuleActivityEditorMediator Mediator
        {
            get
            {
                if (mediator == null)
                    mediator = new ContractBusinessRuleActivityEditorMediator(JSRuntime);
                return mediator;
            }
        }
        ContractBusinessRuleActivityEditorMediator mediator = null;

        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString().ToIdFriendly();

        AlertController alertController;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Mediator.InitBPMN(Id, DiagramXML);
                //Mediator.OnDiagramChange += (caller, args) =>
                //{
                //    EditInProgress = true;
                //    StateHasChanged();
                //};
            }
        }

        //--------------------------------------------------
        //                    REVERT
        //--------------------------------------------------
        DialogWindow revertDialogWindow;
        public async Task RevertAsync()
        {
            await revertDialogWindow.OpenAsync();
        }

        public async Task ConfirmRevertAsync()
        {
            await Mediator.SetDiagramXML(Id, DiagramXML);
            StateHasChanged();
            await revertDialogWindow.CloseAsync();
        }


        //--------------------------------------------------
        //                     SAVE
        //--------------------------------------------------
        DialogWindow saveDialogWindow;
        ContractIntegrityAnalysisResult saveIntegrityResult;
        DMNProcessDiagram diagramToSave;
        public async Task SaveAsync()
        {
            try
            {
                var xml = await Mediator.GetDiagramXML(Id);
                var newDiagram = DMNProcessDiagram.FromXml(xml);
                diagramToSave = newDiagram;

                //Validate
                Contract.ValidatePotentialDiagram(BusinessActivity, newDiagram);
                saveIntegrityResult = Contract.AnalyzeIntegrityWhenReplacedWith(BusinessActivity, newDiagram);

                //Show dialog
                await saveDialogWindow.OpenAsync();
            }
            catch (Exception e)
            {
                alertController.AddAlert("Save unsuccessful: " + e.Message, AlertScheme.Danger);
            }
        }

        protected async Task ConfirmSaveAsync()
        {
            try
            {
                Contract.ReplaceSafely(BusinessActivity, diagramToSave);
                alertController.AddAlert("Save successful", AlertScheme.Success);
                StateHasChanged();
            }
            catch (Exception e)
            {
                alertController.AddAlert("Save confirm unsuccessful: " + e.Message, AlertScheme.Danger);
            }

            await saveDialogWindow.CloseAsync();
        }
    }
}
