export async function getUser() {
    const response = await fetch('client/user');
    return response.json();
}