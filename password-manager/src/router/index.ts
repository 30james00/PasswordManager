import type { IEditPasswordDto } from '@/models/savedPasswordModels';
import ChangePassword from '@/views/ChangePassword.vue';
import EditPassword from '@/views/EditPassword.vue';
import CreatePassword from '@/views/EditPassword.vue';
import Login from '@/views/Login.vue';
import Register from '@/views/Register.vue';
import SavedPasswordList from '@/views/SavedPasswordList.vue';
import type { PropType } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'register',
      component: Register,
    },
    {
      path: '/login',
      name: 'login',
      component: Login,
    },
    {
      path: '/change-password',
      name: 'change-password',
      component: ChangePassword,
    },
    {
      path: '/passwords',
      name: 'password-list',
      component: SavedPasswordList,
    },
    {
      path: '/passwords/create',
      name: 'password-create',
      component: CreatePassword,
    },
    {
      path: '/passwords/edit/:passwordId',
      name: 'password-edit',
      component: EditPassword,
      props: true,
    },
    // {
    //   path: "/about",
    //   name: "about",
    //   // route level code-splitting
    //   // this generates a separate chunk (About.[hash].js) for this route
    //   // which is lazy-loaded when the route is visited.
    //   component: () => import("../views/AboutView.vue"),
    // },
  ],
});

export default router;
