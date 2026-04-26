const TasksComponent = {
    props: ['apiBase'],
    template: `
        <div>
            <h2>Задачи</h2>
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; gap: 10px; flex-wrap: wrap;">
                <div style="display: flex; gap: 10px;">
                    <input type="number" v-model="searchId" placeholder="Поиск по ID" style="padding: 8px; width: 150px;">
                    <button class="btn-search" @click="searchById">Найти</button>
                    <button class="btn-clear" @click="clearSearch">Сбросить</button>
                </div>
                <div style="display: flex; gap: 10px;">
                    <button class="btn-filter" @click="toggleFilter">
                        {{ showInactive
                            ? 'Показать только активные'
                            : 'Показать все' }}
                    </button>
                    <button class="btn-add" @click="openModal()">+ Добавить задачу</button>
                </div>
            </div>
            
            <div class="card">
                <table>
                    <thead>
                        <tr><th>ID</th><th>Название</th><th>Проект</th><th>Активная</th><th>Действия</th></tr>
                    </thead>
                    <tbody>
                        <tr v-for="t in tasks" :key="t.id">
                            <td>{{ t.id }}</td>
                            <td>{{ t.name }}</td>
                            <td>{{ t.projectName }}</td>
                            <td>{{ t.isActive ? 'Да' : 'Нет' }}</td>
                            <td>
                                <button class="btn-edit" @click="openModal(t)">Изменить</button>
                                <button class="btn-delete" @click="deleteTask(t.id)">Удалить</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal" v-if="showModal">
                <div class="modal-content">
                    <h3>{{ editing
                        ? 'Изменить задачу'
                        : 'Новая задача' }}</h3>
                    <div class="form-group">
                        <label>Название</label>
                        <input v-model="form.name" type="text">
                    </div>
                    <div class="form-group">
                        <label>Код проекта</label>
                        <input v-model="projectCode" placeholder="Введите код проекта" type="text" @blur="loadProjectByCode">
                        <small v-if="selectedProjectName" style="color: green;">Проект: {{ selectedProjectName }}</small>
                        <small v-if="projectNotFound" style="color: red;">Проект с таким кодом не найден</small>
                    </div>
                    <div style="display: flex; align-items: center; gap: 10px; margin-bottom: 15px;">
                        <input type="checkbox" v-model="form.isActive" id="isActive">
                        <label for="isActive" style="margin: 0;">Активная</label>
                    </div>
                    <div class="modal-buttons">
                        <button @click="showModal = false">Отмена</button>
                        <button @click="saveTask">Сохранить</button>
                    </div>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            tasks: [],
            projects: [],
            showModal: false,
            editing: false,
            searchId: null,
            showInactive: false,
            projectCode: '',
            selectedProjectName: '',
            projectNotFound: false,
            form: { id: 0, name: '', projectId: null, isActive: true }
        };
    },
    mounted() {
        this.loadTasks();
        this.loadProjects();
    },
    methods: {
        toggleFilter() {
            this.showInactive = !this.showInactive;
            this.loadTasks();
        },
        async loadTasks() {
            const res = await axios.get(`${this.apiBase}/Tasks`, {
                params: { includeInactive: this.showInactive }
            });
            this.tasks = res.data;
        },
        async loadProjects() {
            const res = await axios.get(`${this.apiBase}/Projects`);
            this.projects = res.data;
        },
        async searchById() {
            if (!this.searchId) return;
            try {
                const res = await axios.get(`${this.apiBase}/Tasks/${this.searchId}`);
                this.tasks = [res.data];
            } catch (error) {
                alert('Задача не найдена');
                this.tasks = [];
            }
        },
        clearSearch() {
            this.searchId = null;
            this.loadTasks();
        },
        async loadProjectByCode() {
            if (!this.projectCode) return;
            try {
                const res = await axios.get(`${this.apiBase}/Projects`, {
                    params: { includeInactive: true }
                });
                const project = res.data.find(p => p.code === this.projectCode);
                if (project) {
                    this.selectedProjectName = project.name;
                    this.projectNotFound = false;
                    this.form.projectId = project.id;
                } else {
                    this.selectedProjectName = '';
                    this.projectNotFound = true;
                    this.form.projectId = null;
                }
            } catch (error) {
                this.projectNotFound = true;
            }
        },
        openModal(task = null) {
            if (task) {
                this.form = { ...task };
                this.projectCode = '';
                this.selectedProjectName = '';
                this.projectNotFound = false;
                this.editing = true;
            } else {
                this.form = { id: 0, name: '', projectId: null, isActive: true };
                this.projectCode = '';
                this.selectedProjectName = '';
                this.projectNotFound = false;
                this.editing = false;
            }
            this.showModal = true;
        },
        async saveTask() {
            if (!this.form.projectId) {
                alert('Сначала выберите проект по коду');
                return;
            }
            if (this.editing) {
                await axios.put(`${this.apiBase}/Tasks/${this.form.id}`, this.form);
            } else {
                await axios.post(`${this.apiBase}/Tasks`, this.form);
            }
            this.showModal = false;
            this.loadTasks();
        },
        async deleteTask(id) {
            if (confirm('Удалить задачу?')) {
                try {
                    await axios.delete(`${this.apiBase}/Tasks/${id}`);
                    this.loadTasks();
                } catch (error) {
                    const message = error.response?.data || 'Ошибка при удалении';
                    alert(message);
                }
            }
        }
    }
};