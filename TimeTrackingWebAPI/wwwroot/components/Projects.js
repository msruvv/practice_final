const ProjectsComponent = {
    props: ['apiBase'],
    template: `
        <div>
            <h2>Проекты</h2>
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; gap: 10px;">
                <div style="display: flex; gap: 10px;">
                    <input type="number" v-model="searchId" placeholder="Поиск по ID" style="padding: 8px; width: 150px;">
                    <button class="btn-search" @click="searchById">Найти</button>
                    <button class="btn-clear" @click="clearSearch">Сбросить</button>
                </div>
                <div style="display: flex; gap: 10px;">
                    <button class="btn-filter" @click="toggleFilter">
                        {{ showInactive ? 'Показать только активные' : 'Показать все' }}
                    </button>
                    <button class="btn-add" @click="showModal = true">+ Добавить проект</button>
                </div>
            </div>
            
            <div class="card">
                <table>
                    <thead>
                        <tr><th>ID</th><th>Название</th><th>Код</th><th>Активный</th><th>Действия</th></tr>
                    </thead>
                    <tbody>
                        <tr v-for="p in projects" :key="p.id">
                            <td>{{ p.id }}</td>
                            <td>{{ p.name }}</td>
                            <td>{{ p.code }}</td>
                            <td>{{ p.isActive
                                ? 'Да'
                                : 'Нет' }}</td>
                            <td>
                                <button class="btn-edit" @click="editProject(p)">Изменить</button>
                                <button class="btn-delete" @click="deleteProject(p.id)">Удалить</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal" v-if="showModal">
                <div class="modal-content">
                    <h3>{{ editing
                        ? 'Изменить проект'
                        : 'Новый проект' }}</h3>
                    <div class="form-group">
                        <label>Название</label>
                        <input v-model="form.name" type="text">
                    </div>
                    <div class="form-group">
                        <label>Код</label>
                        <input v-model="form.code" type="text">
                    </div>
                    <div style="display: flex; align-items: center; gap: 10px; margin-bottom: 15px;">
                        <input type="checkbox" v-model="form.isActive" id="isActive">
                        <label for="isActive" style="margin: 0;">Активный</label>
                    </div>
                    <div class="modal-buttons">
                        <button @click="showModal = false">Отмена</button>
                        <button @click="saveProject">Сохранить</button>
                    </div>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            projects: [],
            showModal: false,
            editing: false,
            showInactive: false,
            searchId: null,
            form: { id: 0, name: '', code: '', isActive: true }
        };
    },
    mounted() {
        this.loadProjects();
    },
    methods: {
        toggleFilter() {
            this.showInactive = !this.showInactive;
            this.loadProjects();
        },
        async loadProjects() {
            const res = await axios.get(`${this.apiBase}/Projects`, {
                params: { includeInactive: this.showInactive }
            });
            this.projects = res.data;
        },
        async searchById() {
            if (!this.searchId) return;
            try {
                const res = await axios.get(`${this.apiBase}/Projects/${this.searchId}`);
                this.projects = [res.data];
            } catch (error) {
                alert('Проект не найден');
                this.projects = [];
            }
        },
        clearSearch() {
            this.searchId = null;
            this.loadProjects();
        },
        editProject(p) {
            this.form = { ...p };
            this.editing = true;
            this.showModal = true;
        },
        async saveProject() {
            if (this.editing) {
                await axios.put(`${this.apiBase}/Projects/${this.form.id}`, this.form);
            } else {
                await axios.post(`${this.apiBase}/Projects`, this.form);
            }
            this.showModal = false;
            this.editing = false;
            this.form = { id: 0, name: '', code: '', isActive: true };
            this.loadProjects();
        },
        async deleteProject(id) {
            if (confirm('Удалить проект?')) {
                try {
                    await axios.delete(`${this.apiBase}/Projects/${id}`);
                    this.loadProjects();
                } catch (error) {
                    const message = error.response?.data || 'Ошибка при удалении';
                    alert(message);
                }
            }
        }
    }
};