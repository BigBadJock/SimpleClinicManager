// Chart.js utility functions for statistics dashboard

let chartInstances = {};

function destroyChart(chartId) {
    if (chartInstances[chartId]) {
        chartInstances[chartId].destroy();
        delete chartInstances[chartId];
    }
}

function showFallback(chartId) {
    const canvas = document.getElementById(chartId);
    const fallback = document.getElementById(chartId + 'Fallback');
    if (canvas) canvas.style.display = 'none';
    if (fallback) fallback.style.display = 'block';
}

function hideCanvasIfChartFails(chartId) {
    if (typeof Chart === 'undefined') {
        showFallback(chartId);
        return true;
    }
    return false;
}

window.renderWaitTimeChart = function(data) {
    if (hideCanvasIfChartFails('waitTimeChart')) return;
    
    destroyChart('waitTimeChart');
    
    try {
        const ctx = document.getElementById('waitTimeChart').getContext('2d');
        chartInstances['waitTimeChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(d => d.range),
                datasets: [{
                    label: 'Number of Patients',
                    data: data.map(d => d.count),
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('waitTimeChart');
    }
};

window.renderTreatmentTimeChart = function(data) {
    if (hideCanvasIfChartFails('treatmentTimeChart')) return;
    if (hideCanvasIfChartFails('treatmentTimeChart')) return;
    
    destroyChart('treatmentTimeChart');
    
    try {
        const ctx = document.getElementById('treatmentTimeChart').getContext('2d');
        chartInstances['treatmentTimeChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(d => d.range),
                datasets: [{
                    label: 'Number of Patients',
                    data: data.map(d => d.count),
                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('treatmentTimeChart');
    }
};

window.renderTreatmentTypesChart = function(data) {
    if (hideCanvasIfChartFails('treatmentTypesChart')) return;
    
    destroyChart('treatmentTypesChart');
    
    try {
        const ctx = document.getElementById('treatmentTypesChart').getContext('2d');
        
        // Generate colors for each treatment type matching existing chart styles
        const colors = [
            'rgba(54, 162, 235, 0.8)',   // Blue
            'rgba(255, 99, 132, 0.8)',   // Red
            'rgba(75, 192, 192, 0.8)',   // Teal
            'rgba(255, 206, 86, 0.8)',   // Yellow
            'rgba(153, 102, 255, 0.8)',  // Purple
            'rgba(255, 159, 64, 0.8)',   // Orange
            'rgba(199, 199, 199, 0.8)',  // Grey
            'rgba(83, 102, 255, 0.8)',   // Indigo
            'rgba(255, 99, 255, 0.8)',   // Pink
            'rgba(99, 255, 132, 0.8)'    // Green
        ];
        
        const borderColors = [
            'rgba(54, 162, 235, 1)',
            'rgba(255, 99, 132, 1)',
            'rgba(75, 192, 192, 1)',
            'rgba(255, 206, 86, 1)',
            'rgba(153, 102, 255, 1)',
            'rgba(255, 159, 64, 1)',
            'rgba(199, 199, 199, 1)',
            'rgba(83, 102, 255, 1)',
            'rgba(255, 99, 255, 1)',
            'rgba(99, 255, 132, 1)'
        ];
        
        // Assign colors to each data point, cycling if more than 10 categories
        const backgroundColors = data.map((_, i) => colors[i % colors.length]);
        const borderColorArray = data.map((_, i) => borderColors[i % borderColors.length]);
        
        chartInstances['treatmentTypesChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: data.map(d => d.treatmentName),
                datasets: [{
                    data: data.map(d => d.patientCount),
                    backgroundColor: backgroundColors,
                    borderColor: borderColorArray,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            boxWidth: 12,
                            padding: 10
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.raw || 0;
                                const percentage = data[context.dataIndex]?.percentage?.toFixed(1) || 0;
                                return `${label}: ${value} patients (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('treatmentTypesChart');
    }
};

window.renderCounsellorChart = function(data) {
    destroyChart('counsellorChart');
    
    const ctx = document.getElementById('counsellorChart').getContext('2d');
    chartInstances['counsellorChart'] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(d => d.counsellorName),
            datasets: [{
                label: 'Number of Patients',
                data: data.map(d => d.patientCount),
                backgroundColor: 'rgba(153, 102, 255, 0.6)',
                borderColor: 'rgba(153, 102, 255, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
};

window.renderLanguageChart = function(demographics) {
    destroyChart('languageChart');
    
    const ctx = document.getElementById('languageChart').getContext('2d');
    chartInstances['languageChart'] = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['English First Language', 'Other Language'],
            datasets: [{
                data: [demographics.englishFirstLanguageCount, demographics.otherLanguageCount],
                backgroundColor: [
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)'
                ],
                borderColor: [
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
};

window.renderSurveyChart = function(demographics) {
    destroyChart('surveyChart');
    
    const ctx = document.getElementById('surveyChart').getContext('2d');
    chartInstances['surveyChart'] = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['Survey Returned', 'Survey Not Returned'],
            datasets: [{
                data: [demographics.surveyReturnedCount, demographics.surveyNotReturnedCount],
                backgroundColor: [
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(255, 99, 132, 0.8)'
                ],
                borderColor: [
                    'rgba(75, 192, 192, 1)',
                    'rgba(255, 99, 132, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
};

window.renderTrendsChart = function(data) {
    destroyChart('trendsChart');
    
    const ctx = document.getElementById('trendsChart').getContext('2d');
    chartInstances['trendsChart'] = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(d => d.period),
            datasets: [{
                label: 'Referrals',
                data: data.map(d => d.count),
                backgroundColor: 'rgba(255, 159, 64, 0.2)',
                borderColor: 'rgba(255, 159, 64, 1)',
                borderWidth: 2,
                fill: true,
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
};

