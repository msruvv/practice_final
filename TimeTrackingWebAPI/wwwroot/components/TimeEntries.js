const TimeEntriesComponent = {
    props: ['apiBase'],
    template: `
        <div>
            <h2>Проводки времени</h2>
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; gap: 10px; flex-wrap: wrap;">
                <div style="display: flex; gap: 10px; align-items: center;">
                    <input type="date" v-model="searchDate" style="padding: 8px;">
                    <button class="btn-search" @click="searchByDate">Найти по дате</button>
                    <button class="btn-clear" @click="clearSearch">Сбросить</button>
                </div>
                <button class="btn-add" @click="openModal()">+ Добавить проводку</button>
            </div>
            
            <div class="card">
                <table>
                    <thead>
                        <tr><th>Дата</th><th>Часы</th><th>Описание</th><th>Задача</th><th>Проект</th><th>Действия</th></tr>
                    </thead>
                    <tbody>
                        <tr v-for="e in entries" :key="e.id">
                            <td>{{ formatDate(e.date) }}</td>
                            <td>{{ e.hours }}</td>
                            <td>{{ e.description }}</td>
                            <td>{{ e.taskName }}</td>
                            <td>{{ e.projectName }}</td>
                            <td>
                                <button class="btn-edit" @click="openModal(e)">Изменить</button>
                                <button class="btn-delete" @click="deleteEntry(e.id)">Удалить</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal" v-if="showModal">
                <div class="modal-content">
                    <h3>{{ editing ? 'Изменить проводку' : 'Новая проводка' }}</h3>
                    <div class="form-group">
                        <label>Дата</label>
                        <input v-model="form.date" type="date">
                    </div>
                    <div class="form-group">
                        <label>Часы (0.01 - 24)</label>
                        <input v-model="form.hours" type="number" step="0.5" min="0.01" max="24">
                    </div>
                    <div class="form-group">
                        <label>Описание</label>
                        <input v-model="form.description" type="text">
                    </div>
                    <div class="form-group">
                        <label>ID задачи</label>
                        <input v-model="taskIdInput" placeholder="Введите ID задачи" type="number" @blur="loadTaskById">
                        <small v-if="selectedTaskName" style="color: green;">Задача: {{ selectedTaskName }} (Проект: {{ selectedProjectName }})</small>
                        <small v-if="taskNotFound" style="color: red;">Задача с таким ID не найдена или неактивна</small>
                    </div>
                    <div class="modal-buttons">
                        <button @click="showModal = false">Отмена</button>
                        <button @click="saveEntry">Сохранить</button>
                    </div>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            entries: [],
            tasks: [],
            activeTasks: [],
            showModal: false,
            editing: false,
            canEditTask: true,
            searchDate: null,
            taskIdInput: '',
            selectedTaskName: '',
            selectedProjectName: '',
            taskNotFound: false,
            form: { id: 0, date: '', hours: 1, description: '', taskId: null }
        };
    },
    mounted() {
        this.loadEntries();
        this.loadTasks();
    },
    computed: {
        formatDate() {
            return (date) => new Date(date).toLocaleDateString();
        }
    },
    methods: {
        async loadEntries() {
            const res = await axios.get(`${this.apiBase}/TimeEntries`);
            this.entries = res.data;
        },
        async searchByDate() {
            if (!this.searchDate) return;
            const res = await axios.get(`${this.apiBase}/TimeEntries`, {
                params: { fromDate: this.searchDate, toDate: this.searchDate }
            });
            this.entries = res.data;
        },
        clearSearch() {
            this.searchDate = null;
            this.loadEntries();
        },
        async loadTasks() {
            const res = await axios.get(`${this.apiBase}/Tasks`);
            this.tasks = res.data;
            this.activeTasks = this.tasks.filter(t => t.isActive);
        },
        async loadTaskById() {
            if (!this.taskIdInput) return;
            try {
                const res = await axios.get(`${this.apiBase}/Tasks/${this.taskIdInput}`);
                const task = res.data;
                if (task && task.isActive) {
                    this.selectedTaskName = task.name;
                    this.selectedProjectName = task.projectName;
                    this.taskNotFound = false;
                    this.form.taskId = task.id;
                } else {
                    this.selectedTaskName = '';
                    this.selectedProjectName = '';
                    this.taskNotFound = true;
                    this.form.taskId = null;
                }
            } catch (error) {
                this.selectedTaskName = '';
                this.selectedProjectName = '';
                this.taskNotFound = true;
                this.form.taskId = null;
            }
        },
        async openModal(entry = null) {
            if (entry) {
                this.form = { ...entry };
                this.form.date = new Date(entry.date).toISOString().split('T')[0];
                this.taskIdInput = entry.taskId;
                this.selectedTaskName = entry.taskName;
                this.selectedProjectName = entry.projectName;
                this.editing = true;
                this.canEditTask = entry.canEditTask;
                if (!this.canEditTask) {
                    this.form.taskId = entry.taskId;
                }
            } else {
                this.form = { id: 0, date: new Date().toISOString().split('T')[0], hours: 1, description: '', taskId: null };
                this.taskIdInput = '';
                this.selectedTaskName = '';
                this.selectedProjectName = '';
                this.taskNotFound = false;
                this.editing = false;
                this.canEditTask = true;
            }
            this.showModal = true;
        },
        async saveEntry() {
            if (!this.form.taskId) {
                alert('Сначала выберите задачу по ID');
                return;
            }
            if (this.editing) {
                await axios.put(`${this.apiBase}/TimeEntries/${this.form.id}`, this.form);
            }
            else {
                await axios.post(`${this.apiBase}/TimeEntries`, this.form);
            }
            this.showModal = false;
            this.loadEntries();
        },
        async deleteEntry(id) {
            if (confirm('Удалить проводку?')) {
                await axios.delete(`${this.apiBase}/TimeEntries/${id}`);
                this.loadEntries();
            }
        }
    }
};