using ExamManager.Dto;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ExamManager.Wasm.Pages
{
    public partial class Students
    {
        [Inject]
        public HttpClient Client { get; set; } = default!;
        public StudentDto[] StudentList { get; private set; } = Array.Empty<StudentDto>();
        protected override async Task OnInitializedAsync()
        {
            StudentList = await Client.GetFromJsonAsync<StudentDto[]>("api/students") ?? Array.Empty<StudentDto>();
        }
    }
}
