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
        chartInstances['treatmentTypesChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(d => d.treatmentName),
                datasets: [{
                    label: 'Number of Patients',
                    data: data.map(d => d.patientCount),
                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
                    borderColor: 'rgba(255, 99, 132, 1)',
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

window.renderCareTypesChart = function(data) {
    destroyChart('careTypesChart');
    
    if (!data || data.length === 0) return;
    
    const ctx = document.getElementById('careTypesChart').getContext('2d');
    
    // Define colors for care types - map by care type name for consistency
    const colorMap = {
        'Adjuvant': { bg: 'rgba(46, 204, 113, 0.8)', border: 'rgba(46, 204, 113, 1)' },           // Green
        'Palliative': { bg: 'rgba(155, 89, 182, 0.8)', border: 'rgba(155, 89, 182, 1)' },         // Purple
        'Adjuvant & Palliative': { bg: 'rgba(52, 152, 219, 0.8)', border: 'rgba(52, 152, 219, 1)' }, // Blue
        'Unspecified': { bg: 'rgba(149, 165, 166, 0.8)', border: 'rgba(149, 165, 166, 1)' }       // Gray
    };
    
    const backgroundColors = data.map(d => colorMap[d.careType]?.bg || 'rgba(189, 195, 199, 0.8)');
    const borderColors = data.map(d => colorMap[d.careType]?.border || 'rgba(189, 195, 199, 1)');
    
    chartInstances['careTypesChart'] = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: data.map(d => d.careType),
            datasets: [{
                data: data.map(d => d.patientCount),
                backgroundColor: backgroundColors,
                borderColor: borderColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const value = context.parsed;
                            const percentage = data[context.dataIndex].percentage.toFixed(1);
                            return `${context.label}: ${value} patients (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
};

