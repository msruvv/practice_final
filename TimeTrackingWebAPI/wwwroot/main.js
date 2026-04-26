document.addEventListener('DOMContentLoaded', function () {                                
    Vue.component('projects', ProjectsComponent);
    Vue.component('tasks', TasksComponent);
    Vue.component('time-entries', TimeEntriesComponent);
    Vue.component('reports', ReportsComponent);

    new Vue({
        el: '#app',
        data: {
            activeTab: 'projects',
            apiBase: 'https://localhost:7123/api'
        }
    });
});