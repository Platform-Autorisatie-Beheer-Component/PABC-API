<template>
  <details>
    <summary>
      {{ functionalRole.name }}
    </summary>

    <p v-if="!functionalRole.mappings.length">
      Geen koppelingen gevonden voor deze functionele rol.
    </p>

    <p v-else>Hieronder zie je de koppelingen die op deze functionele rol van toepassing zijn.</p>

    <p>
      <button type="button" class="button secondary" @click="openAddDialog">
        <icon-container icon="plus" /> Toevoegen
      </button>
    </p>

    <item-list
      v-if="functionalRole.mappings.length"
      :items="functionalRole.mappings"
      @delete="openRemoveDialog"
    >
      <template #item="{ item: mapping }">
        <span>
          Is <strong>{{ mapping.name }}</strong>
          {{
            !mapping.isAllEntityTypes
              ? `binnen domein ${mapping.domain}`
              : "voor alle entiteitstypes"
          }}
        </span>
      </template>
    </item-list>
  </details>

  <form-modal
    :is-open="isFormDialogOpen"
    submit-type="create"
    :loading="loading"
    :invalid="invalid"
    @submit="handleAdd"
    @cancel="formDialog.cancel"
  >
    <functional-role-mappings-form
      v-model:mapping="mapping"
      :application-roles="applicationRoles"
      :domains="domains"
    />
  </form-modal>

  <form-modal
    :is-open="isConfirmDialogOpen"
    submit-type="delete"
    :loading="loading"
    @submit="handleRemove"
    @cancel="confirmDialog.cancel"
  >
    <h2>Koppeling verwijderen uit {{ functionalRole.name }}</h2>

    <p>
      Weet je zeker dat je koppeling <em>'{{ selectedMapping?.name }}'</em> wilt verwijderen?
    </p>
  </form-modal>
</template>

<script setup lang="ts">
import { computed, ref, type DeepReadonly } from "vue";
import ItemList from "@/components/ItemList.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import { useDialog } from "@/composables/use-dialog";
import { useFunctionalRoleMappings } from "@/composables/use-functional-role-mappings";
import type {
  ApplicationRole,
  Domain,
  FunctionalRoleMappings,
  Mapping
} from "@/services/pabcService";
import FunctionalRoleMappingsForm from "./FunctionalRoleMappingsForm.vue";

const { functionalRole } = defineProps<{
  functionalRole: DeepReadonly<FunctionalRoleMappings>;
  applicationRoles: DeepReadonly<ApplicationRole[]>;
  domains: DeepReadonly<Domain[]>;
}>();

const emit = defineEmits<{ (e: "refresh"): void }>();

const { loading, invalid, addMapping, removeMapping } = useFunctionalRoleMappings();

const initMapping = (): Mapping => ({
  functionalRoleId: functionalRole.id,
  applicationRoleId: "",
  domainId: "",
  isAllEntityTypes: false
});

const mapping = ref(initMapping());

const selectedMappingId = ref("");

const selectedMapping = computed(() =>
  functionalRole.mappings.find((fr) => fr.id === selectedMappingId.value)
);

const formDialog = useDialog();
const { isOpen: isFormDialogOpen } = formDialog;

const confirmDialog = useDialog();
const { isOpen: isConfirmDialogOpen } = confirmDialog;

const openAddDialog = () => {
  mapping.value = initMapping();

  formDialog.open();
};

const openRemoveDialog = (id: string) => {
  selectedMappingId.value = id;

  confirmDialog.open();
};

const handleAdd = async () => {
  try {
    await addMapping(mapping.value);

    formDialog.confirm();

    emit("refresh");
  } catch {
    // Error displayed via invalid, keep dialog open
  }
};

const handleRemove = async () => {
  await removeMapping(functionalRole.id, selectedMappingId.value);

  confirmDialog.confirm();

  emit("refresh");
};
</script>
