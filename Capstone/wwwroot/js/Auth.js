function validateUsername(input) {
    input.value = input.value.replace(/[0-9]/g, '');
}

function login() {
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    // Perform your authentication logic here

    // Staff Credential
    const staffUsername = 'staff';
    const staffPassword = 'staff123';
    // Admin Credential
    const adminUsername = 'admin';
    const adminPassword = 'admin123';

    if (username === staffUsername && password === staffPassword) {
        // Redirect to the staff dashboard page
        window.location.href = '/Staff/Staff-dashboard';
    } else if (username === adminUsername && password === adminPassword) {
        // Redirect to the admin dashboard page
        window.location.href = 'Admin-Dashboard';
    } else {
        alert('Invalid username or password');
    }
}
