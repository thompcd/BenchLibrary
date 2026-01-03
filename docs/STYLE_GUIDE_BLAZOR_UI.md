# Blazor Server & Web UI Standards

**Framework:** Blazor Server (ASP.NET Core 8)
**Component Library:** MudBlazor
**Rendering:** Server-side Razor Components
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [Component Structure](#component-structure)
2. [Component Naming & Organization](#component-naming--organization)
3. [Code-Behind Pattern](#code-behind-pattern)
4. [Event Handling](#event-handling)
5. [State Management](#state-management)
6. [Form & Input Handling](#form--input-handling)
7. [Styling & CSS](#styling--css)
8. [MudBlazor Usage](#mudblazor-usage)
9. [Data Binding](#data-binding)
10. [Performance](#performance)
11. [Accessibility](#accessibility)

---

## Component Structure

### Single-file component (preferred)
Keep simple components in a single `.razor` file:
```razor
@page "/processes"
@using BenchLibrary.SixSigma.Models
@inject IProcessService ProcessService
@inject ILogger<ProcessesPage> Logger

<PageTitle>Processes</PageTitle>

<h1>Process Management</h1>

@if (processes == null)
{
    <p>Loading...</p>
}
else if (!processes.Any())
{
    <p>No processes found.</p>
}
else
{
    <MudTable Items="@processes" Hover="true" Striped="true">
        <HeaderContent>
            <MudTh>Name</MudTh>
            <MudTh>Created</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd DataLabel="Name">@context.Name</MudTd>
            <MudTd DataLabel="Created">@context.CreatedAt:G</MudTd>
            <MudTd DataLabel="Actions">
                <MudButton Variant="Variant.Text"
                           OnClick="@(() => EditProcess(context.Id))">Edit</MudButton>
            </MudTd>
        </RowTemplate>
    </MudTable>
}

@code {
    private List<ProcessSummary>? processes;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            processes = await ProcessService.GetProcessesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load processes");
        }
    }

    private async Task EditProcess(int id)
    {
        // navigation logic
    }
}
```

### Multi-file component (when code-behind is large)
Split complex components:

**ProcessesPage.razor**
```razor
@page "/processes"
@using BenchLibrary.SixSigma.Models
@inherits ProcessesPageBase

<PageTitle>Processes</PageTitle>

<h1>Process Management</h1>

@if (Processes == null)
{
    <p>Loading...</p>
}
else
{
    <MudTable Items="@Processes">
        <!-- table content -->
    </MudTable>
}
```

**ProcessesPage.razor.cs**
```csharp
using BenchLibrary.SixSigma.Models;
using Microsoft.AspNetCore.Components;

namespace BenchLibrary.Web.Pages;

public class ProcessesPageBase : ComponentBase
{
    [Inject]
    protected IProcessService ProcessService { get; set; } = null!;

    [Inject]
    protected ILogger<ProcessesPage> Logger { get; set; } = null!;

    protected List<ProcessSummary>? Processes { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Processes = await ProcessService.GetProcessesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load processes");
        }
    }
}
```

---

## Component Naming & Organization

### File naming conventions
- **Pages:** `PascalCasePage.razor` (e.g., `ProcessesPage.razor`)
- **Components:** `PascalCaseComponent.razor` (e.g., `MeasurementChart.razor`)
- **Code-behind:** `ComponentName.razor.cs` (e.g., `ProcessesPage.razor.cs`)
- **Layout components:** `PascalCaseLayout.razor` (e.g., `MainLayout.razor`)

### Directory structure
```
Pages/
├── Dashboard.razor
├── ProcessesPage.razor
├── ProcessesPage.razor.cs
├── MeasurementsPage.razor
└── AdminPage.razor

Shared/
├── MainLayout.razor
├── NavMenu.razor
├── Header.razor
└── Footer.razor

Components/
├── Charts/
│   ├── CapabilityChart.razor
│   ├── ControlChart.razor
│   └── HistogramChart.razor
├── Forms/
│   ├── ProcessForm.razor
│   ├── MeasurementInput.razor
│   └── FilterPanel.razor
├── Tables/
│   ├── ProcessTable.razor
│   └── MeasurementTable.razor
└── Common/
    ├── ConfirmationDialog.razor
    ├── ErrorAlert.razor
    └── LoadingSpinner.razor

Layouts/
├── MainLayout.razor
└── AdminLayout.razor

Services/
├── IProcessService.cs
├── ProcessService.cs
├── IMeasurementService.cs
└── MeasurementService.cs
```

---

## Code-Behind Pattern

### Directive usage
```razor
@* Page directives *@
@page "/processes"
@page "/processes/{id:int}"

@* Using statements *@
@using BenchLibrary.SixSigma.Models
@using BenchLibrary.Web.Services

@* Inheritance *@
@inherits ProcessesPageBase

@* Injection *@
@inject IProcessService ProcessService
@inject NavigationManager NavigationManager
@inject ILogger<ProcessesPage> Logger

<h1>Processes</h1>
```

### Property injection vs. attribute injection
```csharp
// ✓ Good - property injection (preferred)
[Inject]
public IProcessService ProcessService { get; set; } = null!;

[Inject]
public NavigationManager NavigationManager { get; set; } = null!;

// ✗ Avoid - attribute injection (harder to test)
@inject IProcessService ProcessService
@inject NavigationManager NavigationManager
```

### Component initialization
```csharp
public class ProcessesPageBase : ComponentBase
{
    private bool _isInitialized;

    protected override async Task OnInitializedAsync()
    {
        // Called on first render, before display
        try
        {
            await LoadDataAsync();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Initialization failed");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Called after render, useful for JS interop
        if (firstRender)
        {
            await InitializeChartAsync();
        }
    }

    // ✗ Avoid OnParametersSet for expensive operations
    // Unless you need to react to parameter changes
    protected override async Task OnParametersSetAsync()
    {
        // Called when parameters change
        if (ProcessId > 0)
        {
            await LoadProcessDetailsAsync(ProcessId);
        }
    }
}
```

---

## Event Handling

### Event handler naming
Use `On` prefix for event handlers:
```csharp
// ✓ Good
private async Task OnSaveClick()
{
    await SaveProcessAsync();
}

private void OnNameChanged(string value)
{
    ProcessName = value;
}

private async Task OnDeleteConfirmed()
{
    await DeleteProcessAsync();
}

// ✗ Avoid - unclear naming
private async Task HandleSave() { }
private void ProcessNameChange(string value) { }
```

### Event handler patterns
```razor
<MudButton OnClick="OnSaveClick">Save</MudButton>
<MudTextField @bind-Value="ProcessName"
              ValueChanged="OnNameChanged" />
<MudSelect T="ProcessType" Value="SelectedType"
           ValueChanged="OnTypeChanged">
</MudSelect>

@code {
    private async Task OnSaveClick()
    {
        // Handle button click
        await SaveAsync();
    }

    private async Task OnNameChanged(string value)
    {
        ProcessName = value;
        await OnProcessChanged();
    }

    private async Task OnTypeChanged(ProcessType type)
    {
        SelectedType = type;
        StateHasChanged(); // notify Blazor if needed
    }
}
```

### Event parameter binding
```razor
@* Passing event parameters *@
<input @onchange="HandleInputChange" />
<button @onclick="@((MouseEventArgs e) => OnMouseClick(e))" />

@code {
    private void HandleInputChange(ChangeEventArgs e)
    {
        var value = e.Value?.ToString() ?? string.Empty;
        ProcessInput(value);
    }

    private void OnMouseClick(MouseEventArgs e)
    {
        Console.WriteLine($"Clicked at {e.ClientX}, {e.ClientY}");
    }
}
```

---

## State Management

### Component-level state
Keep component state simple and localized:
```csharp
public class ProcessFormBase : ComponentBase
{
    [Parameter]
    public int ProcessId { get; set; }

    [Parameter]
    public EventCallback<ProcessModel> OnSubmit { get; set; }

    // State
    private ProcessModel model = new();
    private bool isSaving;
    private string? errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        if (ProcessId > 0)
        {
            model = await LoadProcessAsync(ProcessId);
        }
    }

    private async Task OnSubmitClick()
    {
        try
        {
            isSaving = true;
            await SaveProcessAsync(model);
            await OnSubmit.InvokeAsync(model);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isSaving = false;
        }
    }
}
```

### Cascading parameters
Use cascading parameters for theme or shared context:
```razor
@* MainLayout.razor *@
<CascadingValue Value="this">
    <CascadingValue Value="UserContext">
        @Body
    </CascadingValue>
</CascadingValue>

@code {
    private UserContext? UserContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        UserContext = await GetUserContextAsync();
    }
}

@* Child component *@
@inherits ProcessFormBase

@code {
    [CascadingParameter]
    protected UserContext? UserContext { get; set; }

    private void OnSubmit()
    {
        var userId = UserContext?.UserId;
        // use user context
    }
}
```

### Avoid global state
Don't use static properties or singletons. Use dependency injection instead:
```csharp
// ✗ Bad - global state
public static class GlobalState
{
    public static UserContext? CurrentUser { get; set; }
}

// ✓ Good - injected service
[Inject]
public IUserContextService UserContextService { get; set; } = null!;

private UserContext? currentUser;

protected override async Task OnInitializedAsync()
{
    currentUser = await UserContextService.GetCurrentUserAsync();
}
```

---

## Form & Input Handling

### Two-way binding with validation
```razor
<EditForm Model="@model" OnValidSubmit="OnValidSubmit" OnInvalidSubmit="OnInvalidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <MudTextField @bind-Value="model.ProcessName"
                  Label="Process Name"
                  For="@(() => model.ProcessName)" />

    <MudSelect T="ProcessType"
               @bind-Value="model.Type"
               Label="Process Type"
               For="@(() => model.Type)">
        @foreach (var type in Enum.GetValues<ProcessType>())
        {
            <MudSelectItem Value="@type">@type</MudSelectItem>
        }
    </MudSelect>

    <MudButton ButtonType="ButtonType.Submit" Variant="Variant.Filled">Submit</MudButton>
</EditForm>

@code {
    private ProcessModel model = new();

    private async Task OnValidSubmit()
    {
        // Model is valid, process submission
        await SaveProcessAsync(model);
    }

    private void OnInvalidSubmit()
    {
        // Show error message or allow user to fix
    }
}
```

### Custom validation
```csharp
public class ProcessModel
{
    [Required]
    public string ProcessName { get; set; } = string.Empty;

    [Range(0, 100)]
    public double TargetValue { get; set; }

    [CustomValidation(typeof(ProcessValidator), nameof(ProcessValidator.ValidateSpecification))]
    public ProcessSpecification? Specification { get; set; }
}

public static class ProcessValidator
{
    public static ValidationResult? ValidateSpecification(
        ProcessSpecification? spec,
        ValidationContext context)
    {
        if (spec == null)
            return ValidationResult.Success;

        if (spec.LowerLimit >= spec.UpperLimit)
            return new ValidationResult("Lower limit must be less than upper limit");

        return ValidationResult.Success;
    }
}
```

---

## Styling & CSS

### Scoped CSS
Always use scoped CSS for component styles:
```razor
@* ProcessesPage.razor *@
<div class="processes-container">
    <h1>Processes</h1>
</div>

<style scoped>
    .processes-container {
        padding: 1rem;
        background-color: var(--mud-palette-background);
    }
</style>
```

### CSS class naming
Use BEM-style naming for complex components:
```css
@* ✓ Good - clear hierarchy *@
.process-table { }
.process-table__header { }
.process-table__row { }
.process-table__row--selected { }
.process-table__cell { }
.process-table__cell--highlighted { }

@* ✗ Avoid - ambiguous *@
.table { }
.header { }
.selected { }
```

### MudBlazor CSS variables
Use MudBlazor's CSS variables:
```css
.process-card {
    background-color: var(--mud-palette-surface);
    border-color: var(--mud-palette-divider);
    color: var(--mud-palette-text-primary);
    padding: var(--mud-spacing-3);
    border-radius: var(--mud-default-borderradius);
}
```

### Avoid inline styles
```razor
@* ✗ Avoid *@
<div style="padding: 1rem; background-color: blue;">Content</div>

@* ✓ Good - use CSS class *@
<div class="content-box">Content</div>

<style scoped>
    .content-box {
        padding: 1rem;
        background-color: var(--mud-palette-info);
    }
</style>
```

---

## MudBlazor Usage

### Component library standards
Always use MudBlazor components for consistency:
```razor
@* ✓ Good - MudBlazor components *@
<MudContainer MaxWidth="MaxWidth.Lg">
    <MudCard>
        <MudCardContent>
            <MudText Typo="Typo.h5">Process Details</MudText>
        </MudCardContent>
    </MudCard>
</MudContainer>

@* ✗ Avoid - custom/native HTML *@
<div style="max-width: 1200px; margin: 0 auto;">
    <div style="border: 1px solid #ccc; padding: 1rem;">
        <h2>Process Details</h2>
    </div>
</div>
```

### Common MudBlazor patterns

**Buttons**
```razor
<MudButton Variant="Variant.Filled" Color="Color.Primary">Save</MudButton>
<MudButton Variant="Variant.Outlined" Color="Color.Default">Cancel</MudButton>
<MudButton Variant="Variant.Text" Size="Size.Small">Delete</MudButton>
<MudIconButton Icon="@Icons.Material.Filled.Edit" />
```

**Tables**
```razor
<MudTable Items="@items" Hover="true" Striped="true">
    <ToolBarContent>
        <MudText Typo="Typo.h6">Processes</MudText>
        <MudSpacer />
        <MudButton StartIcon="@Icons.Material.Filled.Add"
                   OnClick="OnAddProcess">Add Process</MudButton>
    </ToolBarContent>
    <HeaderContent>
        <MudTh>Name</MudTh>
        <MudTh>Status</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd DataLabel="Name">@context.Name</MudTd>
        <MudTd DataLabel="Status">@context.Status</MudTd>
    </RowTemplate>
</MudTable>
```

**Dialogs**
```csharp
private async Task OnDeleteClick(int id)
{
    var parameters = new DialogParameters<ConfirmDialog>
    {
        { x => x.Title, "Delete Process?" },
        { x => x.Message, $"Are you sure you want to delete process {id}?" }
    };

    var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete", parameters);
    var result = await dialog.Result;

    if (!result.Canceled)
    {
        await DeleteProcessAsync(id);
    }
}
```

**Notifications**
```csharp
private async Task OnSaveSuccess()
{
    Snackbar.Add("Process saved successfully", Severity.Success);
}

private async Task OnSaveError(string message)
{
    Snackbar.Add($"Error: {message}", Severity.Error);
}
```

---

## Data Binding

### Two-way binding
```razor
@* Use @bind for two-way binding *@
<MudTextField @bind-Value="processName" Label="Process Name" />
<MudSelect T="ProcessType" @bind-Value="selectedType">
    <MudSelectItem Value="ProcessType.Continuous">Continuous</MudSelectItem>
    <MudSelectItem Value="ProcessType.Discrete">Discrete</MudSelectItem>
</MudSelect>

@code {
    private string processName = string.Empty;
    private ProcessType selectedType;
}
```

### Value changed notifications
```razor
@* Use ValueChanged for additional logic *@
<MudTextField Value="@processName"
              ValueChanged="OnProcessNameChanged"
              Label="Process Name" />

@code {
    private string processName = string.Empty;

    private async Task OnProcessNameChanged(string value)
    {
        processName = value;
        // Perform additional actions
        await ValidateNameAsync(value);
    }
}
```

### Collection binding
```razor
<MudCheckBox T="bool" Checked="@selectedStatuses.Contains(ProcessStatus.Active)"
             CheckedChanged="@((bool c) => OnStatusToggled(ProcessStatus.Active, c))">
    Active
</MudCheckBox>

@code {
    private HashSet<ProcessStatus> selectedStatuses = new();

    private void OnStatusToggled(ProcessStatus status, bool isChecked)
    {
        if (isChecked)
            selectedStatuses.Add(status);
        else
            selectedStatuses.Remove(status);
    }
}
```

---

## Performance

### Prevent unnecessary re-renders
```csharp
// ✓ Good - prevent re-render when params don't change
[Parameter]
public int ProcessId { get; set; }

private int _previousProcessId = -1;

protected override async Task OnParametersSetAsync()
{
    if (ProcessId == _previousProcessId)
        return; // Skip re-load if ProcessId didn't change

    _previousProcessId = ProcessId;
    await LoadProcessAsync(ProcessId);
}

// ✗ Avoid - re-renders on every parameter check
protected override async Task OnParametersSetAsync()
{
    await LoadProcessAsync(ProcessId); // loads every time, even if ProcessId unchanged
}
```

### Virtualization for large lists
```razor
<Virtualize Items="@processes" Context="process">
    <div class="process-item">
        <p>@process.Name</p>
    </div>
</Virtualize>

@code {
    private List<ProcessModel>? processes;

    protected override async Task OnInitializedAsync()
    {
        processes = await ProcessService.GetAllProcessesAsync();
    }
}
```

### Lazy loading components
```razor
@* MainLayout.razor *@
<MudContainer>
    <NavMenu />
    @Body
    <ErrorBoundary>
        @if (showFooter)
        {
            <Footer />
        }
    </ErrorBoundary>
</MudContainer>

@code {
    private bool showFooter;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(500); // Delay non-critical footer
            showFooter = true;
            StateHasChanged();
        }
    }
}
```

### Error boundaries
```razor
<ErrorBoundary @ref="errorBoundary">
    <ChildContent>
        <ProcessChart ProcessId="@ProcessId" />
    </ChildContent>
    <ErrorContent>
        <MudAlert Severity="Severity.Error">
            Failed to load process chart. Please refresh the page.
        </MudAlert>
    </ErrorContent>
</ErrorBoundary>

@code {
    private ErrorBoundary? errorBoundary;

    private void OnError()
    {
        errorBoundary?.Recover();
    }
}
```

---

## Accessibility

### ARIA labels
```razor
<MudButton aria-label="Add new process"
           StartIcon="@Icons.Material.Filled.Add">
    Add
</MudButton>

<input type="text" aria-label="Search processes" placeholder="Search..." />

<div role="alert" aria-live="polite">
    @errorMessage
</div>
```

### Form accessibility
```razor
<label for="process-name">Process Name</label>
<input id="process-name" type="text" @bind="processName" />

@* Or with MudBlazor *@
<MudTextField @bind-Value="processName"
              Label="Process Name"
              HelperText="Enter the process identifier" />
```

### Keyboard navigation
```razor
<MudButton OnClick="OnSaveClick"
           onclick="javascript:void(0);"
           onkeydown="@((KeyboardEventArgs e) => OnKeyDown(e))">
    Save
</MudButton>

@code {
    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnSaveClick();
        }
    }
}
```

### Color contrast
Ensure sufficient color contrast using MudBlazor's color palette:
```css
@* Use MudBlazor colors for guaranteed contrast *@
color: var(--mud-palette-text-primary);
background: var(--mud-palette-surface);
```

---

## Key Rules Summary

✓ **DO:**
- Use PascalCase for component and page names
- Use scoped CSS for component styles
- Implement proper error handling and loading states
- Use MudBlazor components for UI consistency
- Inject dependencies via properties
- Use async/await for all async operations
- Implement proper form validation
- Use cascading parameters for shared context
- Add ARIA labels for accessibility

✗ **DON'T:**
- Mix file types (e.g., multiple components in one file)
- Use `@inject` for dependencies in code-behind
- Inline large amounts of styling
- Create overly complex components (split if >200 lines)
- Block on async operations
- Assume HTML elements exist (use MudBlazor)
- Ignore error states in components
- Use global state management
- Skip accessibility features

