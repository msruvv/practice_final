const ReportsComponent = {
    props: ['apiBase'],
    template: `
        <div>
            <h2>Отчеты</h2>
            
            <div class="card">
                <h3>Отчет за день</h3>
                <div class="filter-group">
                    <input type="date" v-model="selectedDate">
                    <button @click="loadDayReport">Показать</button>
                </div>

                <div v-if="dayReport">
                    <div class="report-header">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div style="text-align: left;">
                                <strong>Дата:</strong> {{ dayReport.date.split('T')[0] }}<br>
                                <strong>Всего часов:</strong> {{ dayReport.totalHours }}<br>
                                <strong>Сообщение:</strong> {{ dayReport.message }}
                            </div>
                            <div class="sticker" :class="'sticker-' + dayReport.stickerColor.toLowerCase()"></div>
                       </div>
                    </div>
                    
                    <div v-if="dayReport.entries && dayReport.entries.length">
                        <h4>Проводки:</h4>
                        <table>
                            <thead>
                                <tr><th>Задача</th><th>Проект</th><th>Часы</th><th>Описание</th></tr>
                            </thead>
                            <tbody>
                                <tr v-for="e in dayReport.entries" :key="e.id">
                                    <td>{{ e.taskName }}</td>
                                    <td>{{ e.projectName }}</td>
                                    <td>{{ e.hours }}</td>
                                    <td>{{ e.description }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div v-else-if="dayReport === null && dayReportLoaded">
                    <p>Нет данных за выбранный день</p>
                </div>
            </div>

            <div class="card">
                <h3>Проводки за неделю</h3>
                <div class="filter-group">
                    <input type="date" v-model="weekDate">
                    <button @click="loadWeekReport">Показать</button>
                </div>
                <div v-if="weekEntries.length">
                    <table>
                        <thead><tr><th>Дата</th><th>Задача</th><th>Проект</th><th>Часы</th><th>Описание</th></tr></thead>
                        <tbody>
                            <tr v-for="e in weekEntries" :key="e.id">
                                <td>{{ formatDate(e.date) }}</td>
                                <td>{{ e.taskName }}</td>
                                <td>{{ e.projectName }}</td>
                                <td>{{ e.hours }}</td>
                                <td>{{ e.description }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div v-else-if="weekEntriesLoaded && weekEntries.length === 0">
                    <p>Нет проводок за выбранную неделю</p>
                </div>
            </div>

            <div class="card">
                <h3>Проводки за месяц</h3>
                <div class="filter-group">
                    <input type="number" v-model="monthYear" placeholder="Год" style="width:80px">
                    <select v-model="monthNum">
                        <option v-for="m in months" :value="m.num">{{ m.name }}</option>
                    </select>
                    <button @click="loadMonthReport">Показать</button>
                </div>
                <div v-if="monthEntries.length">
                    <table>
                        <thead><tr><th>Дата</th><th>Задача</th><th>Проект</th><th>Часы</th><th>Описание</th></tr></thead>
                        <tbody>
                            <tr v-for="e in monthEntries" :key="e.id">
                                <td>{{ formatDate(e.date) }}</td>
                                <td>{{ e.taskName }}</td>
                                <td>{{ e.projectName }}</td>
                                <td>{{ e.hours }}</td>
                                <td>{{ e.description }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div v-else-if="monthEntriesLoaded && monthEntries.length === 0">
                    <p>Нет проводок за выбранный месяц</p>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            selectedDate: new Date().toISOString().split('T')[0],
            dayReport: null,
            dayReportLoaded: false,
            weekDate: new Date().toISOString().split('T')[0],
            weekEntries: [],
            weekEntriesLoaded: false,
            monthYear: new Date().getFullYear(),
            monthNum: new Date().getMonth() + 1,
            monthEntries: [],
            monthEntriesLoaded: false,
            months: [
                { num: 1, name: 'Январь' }, { num: 2, name: 'Февраль' },
                { num: 3, name: 'Март' }, { num: 4, name: 'Апрель' },
                { num: 5, name: 'Май' }, { num: 6, name: 'Июнь' },
                { num: 7, name: 'Июль' }, { num: 8, name: 'Август' },
                { num: 9, name: 'Сентябрь' }, { num: 10, name: 'Октябрь' },
                { num: 11, name: 'Ноябрь' }, { num: 12, name: 'Декабрь' }
            ]
        };
    },
    methods: {
        formatDate(date) {
            if (!date) return '';
            return new Date(date).toLocaleDateString();
        },
        async loadDayReport() {
            try {
                const res = await axios.get(`${this.apiBase}/Reports/day`, {
                    params: { date: this.selectedDate }
                });
                console.log('Day report:', res.data);
                this.dayReport = res.data;
                this.dayReportLoaded = true;
            } catch (error) {
                console.error('Ошибка загрузки отчета за день:', error);
                alert('Ошибка: ' + (error.response?.data || error.message));
            }
        },
        async loadWeekReport() {
            try {
                const res = await axios.get(`${this.apiBase}/Reports/week`, {
                    params: { date: this.weekDate }
                });
                console.log('Week report:', res.data);
                this.weekEntries = Array.isArray(res.data) ? res.data : [];
                this.weekEntriesLoaded = true;
            } catch (error) {
                console.error('Ошибка загрузки отчета за неделю:', error);
                alert('Ошибка: ' + (error.response?.data || error.message));
            }
        },
        async loadMonthReport() {
            try {
                const res = await axios.get(`${this.apiBase}/Reports/month`, {
                    params: { year: this.monthYear, month: this.monthNum }
                });
                console.log('Month report:', res.data);
                this.monthEntries = Array.isArray(res.data) ? res.data : [];
                this.monthEntriesLoaded = true;
            } catch (error) {
                console.error('Ошибка загрузки отчета за месяц:', error);
                alert('Ошибка: ' + (error.response?.data || error.message));
            }
        }
    },
    mounted() {
        this.loadDayReport();
    }
};